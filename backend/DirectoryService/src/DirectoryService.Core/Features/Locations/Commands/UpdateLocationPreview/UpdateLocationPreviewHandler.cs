using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.HttpCommunication;
using DirectoryService.Locations.Requests;
using DirectoryService.ValueObjects.Location;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.UpdateLocationPreview;

public sealed class UpdateLocationPreviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/locations/{locationId:guid}/preview", async Task<EndpointResult<Guid>> (
            [FromRoute] Guid locationId,
            [FromBody] UpdateLocationPreviewRequest request,
            [FromServices] UpdateLocationPreviewHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLocationPreviewCommand(locationId, request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public record UpdateLocationPreviewCommand(Guid LocationId, UpdateLocationPreviewRequest Request) : ICommand;

public class UpdateLocationPreviewHandler : ICommandHandler<Guid, UpdateLocationPreviewCommand>
{
    private readonly ILogger<UpdateLocationPreviewHandler> _logger;
    private readonly ITransactionManager _transactionManager;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IFileCommunicationService _fileCommunicationService;

    public UpdateLocationPreviewHandler(
        IFileCommunicationService fileCommunicationService,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        ILogger<UpdateLocationPreviewHandler> logger)
    {
        _fileCommunicationService = fileCommunicationService;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdateLocationPreviewCommand command,
        CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository
            .GetBy(l => l.Id == new LocationId(command.LocationId), cancellationToken);

        if (locationResult.IsFailure)
            return locationResult.Error.ToErrors();

        if (command.Request.PreviewId == null)
        {
            return Error.NotFound("invalid.data", "preview Id is null").ToErrors();
        }

        var result = await _fileCommunicationService
            .CheckMediaAssetExists(command.Request.PreviewId.Value, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError(
                "File Service asset check failed for {PreviewId}: {Error}",
                command.Request.PreviewId,
                result.Error);
            return result.Error;
        }

        if (!result.Value.Exists)
        {
            return Error.NotFound(
                "preview.not.found",
                "Preview asset was not found in File Service").ToErrors();
        }

        locationResult.Value.UpdatePreviewId(command.Request.PreviewId);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created location preview with id: {Id}", locationResult.Value.Id);

        return locationResult.Value.Id.Value;

    }
}