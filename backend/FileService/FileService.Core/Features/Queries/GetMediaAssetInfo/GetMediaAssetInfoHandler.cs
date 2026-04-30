using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Models;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetMediaAssetInfo;

public class GetMediaAssetInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{mediaAssetId:guid}", async Task<EndpointResult<MediaAssetInfoDto?>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] GetMediaAssetInfoHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMediaAssetInfoHandlerQuery(mediaAssetId);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetMediaAssetInfoHandler : IQueryHandler<MediaAssetInfoDto?, GetMediaAssetInfoHandlerQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadDbContext _readDbContext;

    public GetMediaAssetInfoHandler(
        IS3Provider s3Provider,
        IReadDbContext readDbContext)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
    }

    public async Task<Result<MediaAssetInfoDto?, Errors>> Handle(
        GetMediaAssetInfoHandlerQuery query,
        CancellationToken cancellationToken)
    {
        var mediaAsset = await _readDbContext.MediaAssetsRead
            .FirstOrDefaultAsync(
                m => m.Id == query.MediaAssetId
                            && m.Status != MediaStatus.DELETED, cancellationToken);

        if (mediaAsset == null)
        {
            return Result.Success<MediaAssetInfoDto?, Errors>(null);
        }

        string? url = null;

        if (mediaAsset.Status is MediaStatus.READY)
        {
            (_, bool isFailure, MediaUrl? presignedUrl, Error? error) = await _s3Provider.GenerateDownloadUrlAsync(mediaAsset.RawKey);
            if (isFailure)
                return error.ToErrors();

            url = presignedUrl.PresignedUrl;
        }

        return new MediaAssetInfoDto(
            query.MediaAssetId,
            mediaAsset.Status.GetDisplayName(),
            mediaAsset.AssetType.GetDisplayName(),
            mediaAsset.CreatedAt,
            mediaAsset.UpdatedAt,
            new FileInfoDto(
                mediaAsset.MediaData.FileName.Name,
                mediaAsset.MediaData.ContentType.Value,
                mediaAsset.MediaData.Size),
            url);
    }
}