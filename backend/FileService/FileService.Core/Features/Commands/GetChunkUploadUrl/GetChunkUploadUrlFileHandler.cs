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

namespace DirectoryService.Features.Commands.GetChunkUploadUrl;

public class GetChunkUploadUrlFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart/url", async Task<EndpointResult<GetChunkUploadUrlFileResponse>>(
            [FromBody] GetChunkUploadUrlFileRequest request,
            [FromServices] GetChunkUploadUrlFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GetChunkUploadUrlFileCommand(request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetChunkUploadUrlFileHandler : ICommandHandler<GetChunkUploadUrlFileResponse, GetChunkUploadUrlFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<GetChunkUploadUrlFileHandler> _logger;
    private readonly IValidator<GetChunkUploadUrlFileCommand> _validator;

    public GetChunkUploadUrlFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<GetChunkUploadUrlFileHandler> logger,
        IValidator<GetChunkUploadUrlFileCommand> validator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetChunkUploadUrlFileResponse, Errors>> Handle(
        GetChunkUploadUrlFileCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {command}", command);
            return validationResult.ToErrors();
        }

        var request = command.Request;

        (_, bool isFailure, MediaAsset? mediaAsset, Error? error) = await _mediaAssetRepository
            .GetBy(m => m.Id == request.MediaAssetId, cancellationToken);

        if (isFailure)
        {
            _logger.LogInformation("Can`t find the media asset {mediaAssetId}", request.MediaAssetId);
            return error.ToErrors();
        }

        if (request.PartNumber > mediaAsset.MediaData.ExpectedChunksCount)
        {
            _logger.LogInformation("Part number is out of bounds");
            return GeneralErrors.General.ValueIsInvalid("Part number is out of bounds").ToErrors();
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