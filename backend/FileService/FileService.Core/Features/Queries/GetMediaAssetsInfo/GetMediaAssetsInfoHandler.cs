using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Models;
using DirectoryService.Requests;
using DirectoryService.Responses;
using DirectoryService.VOs;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetMediaAssetsInfo;

public class GetMediaAssetInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/batch", async Task<EndpointResult<GetMediaAssetsInfoResponse>>(
            [FromBody] GetMediaAssetsInfoRequest request,
            [FromServices] GetMediaAssetsInfoHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMediaAssetsInfoHandlerQuery(request);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetMediaAssetsInfoHandler : IQueryHandler<GetMediaAssetsInfoResponse, GetMediaAssetsInfoHandlerQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadDbContext _readDbContext;

    public GetMediaAssetsInfoHandler(
        IS3Provider s3Provider,
        IReadDbContext readDbContext)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetMediaAssetsInfoResponse, Errors>> Handle(
        GetMediaAssetsInfoHandlerQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.Request.MediaAssetIds.Any())
            return new GetMediaAssetsInfoResponse([]);

        List<MediaAsset> mediaAssets = await _readDbContext.MediaAssetsQuery
            .Where(m => query.Request.MediaAssetIds.Contains(m.Id)
                        && m.Status != MediaStatus.DELETED)
                            .ToListAsync(cancellationToken);

        var readyMediaAssets = mediaAssets.Where(m => m.Status == MediaStatus.READY).ToList();
        List<StorageKey> storageKeys = readyMediaAssets.Select(s => s.RawKey).ToList();

        (_, bool isFailure, IReadOnlyList<MediaUrl>? mediaUrls, Errors? error) = await _s3Provider
            .GenerateDownloadUrlsAsync(storageKeys);

        if (isFailure)
            return error;

        Dictionary<StorageKey, string> urlsDict = mediaUrls.ToDictionary(url => url.StorageKey, url => url.PresignedUrl);

        var results = new List<MediaAssetsInfoDto>();

        foreach (MediaAsset mediaAsset in mediaAssets)
        {
            string? downloadUrl = null;

            if(urlsDict.TryGetValue(mediaAsset.RawKey, out string? url))
                downloadUrl = url;

            var mediaAssetDto = new MediaAssetsInfoDto(
                    mediaAsset.Id,
                    mediaAsset.Status.GetDisplayName(),
                    downloadUrl);

            results.Add(mediaAssetDto);
        }

        return new GetMediaAssetsInfoResponse(results);
    }
}