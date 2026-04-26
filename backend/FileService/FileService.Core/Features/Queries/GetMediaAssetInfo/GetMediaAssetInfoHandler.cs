using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Responses;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetMediaAssetInfo;

public class GetMediaAssetInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/download/{mediaAssetId:guid}", async Task<EndpointResult<GetMediaAssetInfoResponse>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] GetMediaAssetInfoHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMediaAssetInfoHandlerQuery(mediaAssetId);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetMediaAssetInfoHandler : IQueryHandler<GetMediaAssetInfoResponse, GetMediaAssetInfoHandlerQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<GetMediaAssetInfoHandlerQuery> _validator;
    private readonly ILogger<GetMediaAssetInfoHandler> _logger;
    private readonly IMediaAssetsRepository _mediaAssetRepository;

    public GetMediaAssetInfoHandler(
        IS3Provider s3Provider,
        IValidator<GetMediaAssetInfoHandlerQuery> validator,
        ILogger<GetMediaAssetInfoHandler> logger,
        IMediaAssetsRepository mediaAssetsRepository)
    {
        _s3Provider = s3Provider;
        _validator = validator;
        _logger = logger;
        _mediaAssetRepository = mediaAssetsRepository;
    }

    public async Task<Result<GetMediaAssetInfoResponse, Errors>> Handle(
        GetMediaAssetInfoHandlerQuery query,
        CancellationToken cancellationToken)
    {
        (_, bool isFailure, MediaAsset? mediaAsset, Error? error) = await _mediaAssetRepository
            .GetBy(m => m.Id == query.MediaAssetId && m.Status != MediaStatus.DELETED, cancellationToken);

        if (isFailure)
        {
            _logger.LogInformation("Can`t find the media asset {mediaAssetId}", query.MediaAssetId);
            return error.ToErrors();
        }

        if (mediaAsset.Status is MediaStatus.READY)
        {
            var generateResult = await _s3Provider.GenerateDownloadUrlAsync(mediaAsset.RawKey);
            if (generateResult.IsFailure)
            {
                _logger.LogInformation("Can`t generate the download url {downloadUrl}", generateResult.Value);
                return generateResult.Error.ToErrors();
            }
        }

        return new GetMediaAssetInfoResponse(query.MediaAssetId);
    }
}