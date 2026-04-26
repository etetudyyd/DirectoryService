using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Requests;
using DirectoryService.Responses;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetChunkUploadUrl;

public class GetChunkUploadUrlFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/multipart/url", async Task<EndpointResult<GetChunkUploadUrlFileResponse>>(
            [FromBody] GetChunkUploadUrlFileRequest request,
            [FromServices] GetChunkUploadUrlFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetChunkUploadUrlFileQuery(request);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetChunkUploadUrlFileHandler : IQueryHandler<GetChunkUploadUrlFileResponse, GetChunkUploadUrlFileQuery>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<GetChunkUploadUrlFileHandler> _logger;
    private readonly IValidator<GetChunkUploadUrlFileQuery> _validator;

    public GetChunkUploadUrlFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<GetChunkUploadUrlFileHandler> logger,
        IValidator<GetChunkUploadUrlFileQuery> validator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetChunkUploadUrlFileResponse, Errors>> Handle(
        GetChunkUploadUrlFileQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {command}", query);
            return validationResult.ToErrors();
        }

        var request = query.Request;

        (_, bool isFailure, MediaAsset? mediaAsset, Error? error) = await _mediaAssetRepository
            .GetBy(m => m.Id == request.MediaAssetId, cancellationToken);

        if (isFailure)
        {
            _logger.LogInformation("Can`t find the media asset {mediaAssetId}", request.MediaAssetId);
            return error.ToErrors();
        }

        var generateUrlResult = await _s3Provider
            .GenerateChunkUploadUrl(
                mediaAsset.RawKey,
                request.UploadId,
                request.PartNumber);

        if (generateUrlResult.IsFailure)
        {
            _logger.LogInformation("Can`t generate the url {generateUrlResult}", generateUrlResult.Value);
            return generateUrlResult.Error.ToErrors();
        }

        return new GetChunkUploadUrlFileResponse(generateUrlResult.Value, request.PartNumber);
    }
}