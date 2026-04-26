using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.Download;

public class DownloadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/download/{id:guid}", async Task<EndpointResult<string>>(
            [FromRoute] Guid id,
            [FromServices] DownloadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new DownloadFileQuery(id);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class DownloadFileHandler : IQueryHandler<string, DownloadFileQuery>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<DownloadFileHandler> _logger;

    public DownloadFileHandler(
        IMediaAssetsRepository mediaAssetRepository,
        IS3Provider s3Provider,
        ILogger<DownloadFileHandler> logger)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public async Task<Result<string, Errors>> Handle(
        DownloadFileQuery query,
        CancellationToken cancellationToken)
    {
        var fileId = query.FileId;
        var mediaAssetResult = await _mediaAssetRepository
            .GetBy(x => x.Id == fileId, cancellationToken);

        if (mediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Failed to find file with id:{fileId}", fileId);
            return mediaAssetResult.Error.ToErrors();
        }

        var mediaAsset = mediaAssetResult.Value;

        var downloadResult = await _s3Provider.DownloadFileAsync(mediaAsset.RawKey, cancellationToken);

        if (downloadResult.IsFailure)
        {
            _logger.LogInformation("Failed download file with id:{fileId}", fileId);
            return downloadResult.Error.ToErrors();
        }

        _logger.LogInformation("Download file with id:{fileId}", fileId);

        return downloadResult.Value;
    }
}