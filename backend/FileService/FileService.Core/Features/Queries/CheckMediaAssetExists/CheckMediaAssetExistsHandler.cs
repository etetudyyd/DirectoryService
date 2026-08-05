using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Responses;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.CheckMediaAssetExists;

public sealed class CheckMediaAssetExistsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{mediaAssetId:guid}/exists",  async Task<EndpointResult<CheckMediaAssetExistsResponse>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] CheckMediaAssetExistsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new CheckMediaAssetExistsQuery(mediaAssetId);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public record CheckMediaAssetExistsQuery(Guid MediaAssetId) : IQuery;

public class CheckMediaAssetExistsHandler : IQueryHandler<CheckMediaAssetExistsResponse, CheckMediaAssetExistsQuery>
{
    private readonly ILogger<CheckMediaAssetExistsHandler> _logger;
    private readonly IReadDbContext _readDbContext;

    public CheckMediaAssetExistsHandler(ILogger<CheckMediaAssetExistsHandler> logger, IReadDbContext readDbContext)
    {
        _logger = logger;
        _readDbContext = readDbContext;
    }

    public async Task<Result<CheckMediaAssetExistsResponse, Errors>> Handle(CheckMediaAssetExistsQuery query, CancellationToken cancellationToken)
    {
        bool exists = await _readDbContext.MediaAssetsRead.AnyAsync(m => m.Id == query.MediaAssetId, cancellationToken);

        _logger.LogDebug($"CheckMediaAssetExistsHandler.Handle() returns: {exists}");

        return new CheckMediaAssetExistsResponse(exists);
    }
}