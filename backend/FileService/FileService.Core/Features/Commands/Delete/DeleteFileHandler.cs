using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.Delete;

public class DeleteFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files/delete/{id:guid}", async Task<EndpointResult<Guid>>(
            [FromRoute] Guid id,
            [FromServices] DeleteFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFileCommand(id);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class DeleteFileHandler : ICommandHandler<Guid, DeleteFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<DeleteFileHandler> _logger;

    public DeleteFileHandler(
        IMediaAssetsRepository mediaAssetRepository,
        IS3Provider s3Provider,
        ILogger<DeleteFileHandler> logger)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeleteFileCommand command,
        CancellationToken cancellationToken)
    {
        var mediaAssetResult = await _mediaAssetRepository.GetBy(
            x => x.Id == command.FileId,
            cancellationToken);

        if (mediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Can`t find file to delete: {command.FileId}", command.FileId);
            return mediaAssetResult.Error.ToErrors();
        }

        var mediaAsset = mediaAssetResult.Value;

        var deleteFileResult = await _s3Provider.DeleteFileAsync(mediaAsset.RawKey, cancellationToken);

        if (deleteFileResult.IsFailure)
        {
            _logger.LogInformation("Failed to delete file: {command.FileId}", command.FileId);
            return deleteFileResult.Error.ToErrors();
        }

        mediaAsset.MarkDeleted(DateTime.UtcNow);

        await _mediaAssetRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Deleted file {command.FileId}");

        return mediaAsset.Id;
    }
}