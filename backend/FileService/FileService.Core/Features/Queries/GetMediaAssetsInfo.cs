using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.Dtos;
using DirectoryService.FilesStorage;
using DirectoryService.HttpCommunication;
using DirectoryService.Models;
using DirectoryService.Requests;
using DirectoryService.Responses;
using DirectoryService.VOs;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries;

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

public record GetMediaAssetsInfoHandlerQuery(GetMediaAssetsInfoRequest Request) : IQuery;

public class GetMediaAssetsInfoHandler : IQueryHandler<GetMediaAssetsInfoResponse, GetMediaAssetsInfoHandlerQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadDbContext _readDbContext;
    private readonly HybridCache _cache;
    private readonly FileStorageOptions _fileStorageOptions;

    public GetMediaAssetsInfoHandler(
        IS3Provider s3Provider,
        IReadDbContext readDbContext,
        HybridCache cache,
        IOptions<FileStorageOptions> fileStorageOptions)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
        _cache = cache;
        _fileStorageOptions = fileStorageOptions.Value;
    }

    public async Task<Result<GetMediaAssetsInfoResponse, Errors>> Handle(
        GetMediaAssetsInfoHandlerQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.Request.MediaAssetIds.Any())
            return new GetMediaAssetsInfoResponse([]);

        List<MediaAsset> mediaAssets = await _readDbContext.MediaAssetsRead
            .Where(m => query.Request.MediaAssetIds.Contains(m.Id)
                        && m.Status != MediaStatus.DELETED)
                            .ToListAsync(cancellationToken);

        var readyMediaAssets = mediaAssets.Where(m => m.Status == MediaStatus.READY).ToList();
        List<StorageKey> storageKeys = readyMediaAssets.Select(s => s.RawKey).ToList();

        Dictionary<StorageKey, string> presignedUrls = await GetPresignedUrlsFromCacheAsync(storageKeys, cancellationToken);

        var results = new List<MediaAssetsInfoDto>();

        foreach (MediaAsset mediaAsset in mediaAssets)
        {
            string? downloadUrl = null;

            if(presignedUrls.TryGetValue(mediaAsset.RawKey, out string? url))
                downloadUrl = url;

            var mediaAssetDto = new MediaAssetsInfoDto(
                    mediaAsset.Id,
                    mediaAsset.Status.GetDisplayName(),
                    downloadUrl);

            results.Add(mediaAssetDto);
        }

        return new GetMediaAssetsInfoResponse(results);
    }

    private async Task<Dictionary<StorageKey, string>> GetPresignedUrlsFromCacheAsync(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken cancellationToken)
    {
        List<StorageKey> keys = storageKeys.ToList();

        if (keys.Count == 0)
            return [];

        IEnumerable<Task<(StorageKey storageKey, string? url)>> cachedUrlsTasks = keys.Select(async key =>
        {
            string? url = await _cache.GetOrCreateAsync<string?>(
                key.Value,
                factory: _ => ValueTask.FromResult<string?>(null),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(_fileStorageOptions.DownloadUrlExpirationHours * 0.5) },
                cancellationToken: cancellationToken);

            return (key, url);
        });

        (StorageKey storageKey, string? url)[] cachedUrls = await Task.WhenAll(cachedUrlsTasks);
        var result = new Dictionary<StorageKey, string>();
        var keysToGenerate = new List<StorageKey>();

        foreach ((StorageKey key, string? url) in cachedUrls)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                result[key] = url;
            }
            else
            {
                keysToGenerate.Add(key);
            }
        }

        if (!keysToGenerate.Any())
            return result;

        Result<IReadOnlyList<MediaUrl>, Errors> mediaUrls = await _s3Provider.GetDownloadUrlsAsync(keysToGenerate);

        if(mediaUrls.IsFailure)
            return result;

        IEnumerable<Task> setTasks = mediaUrls.Value.Select(async mediaUrl =>
        {
            result[mediaUrl.StorageKey] = mediaUrl.PresignedUrl;

            await _cache.SetAsync(
                key: mediaUrl.StorageKey.Value,
                value: mediaUrl.PresignedUrl,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(_fileStorageOptions.DownloadUrlExpirationHours * 0.5) },
                cancellationToken: cancellationToken);
        });

        await Task.WhenAll(setTasks);

        return result;
    }
}

