using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Requests;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.CancelMultipartUpload;

public class CancelMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart/cancel", async Task<EndpointResult>(
            [FromBody] CancelMultipartUploadFileRequest request,
            [FromServices] CancelMultipartUploadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelMultipartUploadFileCommand(request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class CancelMultipartUploadFileHandler : ICommandHandler<CancelMultipartUploadFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<CancelMultipartUploadFileHandler> _logger;
    private readonly IValidator<CancelMultipartUploadFileCommand> _validator;

    public CancelMultipartUploadFileHandler(
        IMediaAssetsRepository mediaAssetRepository,
        IS3Provider s3Provider,
        ILogger<CancelMultipartUploadFileHandler> logger,
        IValidator<CancelMultipartUploadFileCommand> validator)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _s3Provider = s3Provider;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Errors>> Handle(
        CancelMultipartUploadFileCommand command,
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

        var cancelUploadResult = await _s3Provider
            .AbortMultipartUploadAsync(mediaAsset.RawKey, request.UploadId, cancellationToken);
        if (cancelUploadResult.IsFailure)
        {
            _logger.LogInformation("Can`t cancel uploading the media asset {mediaAssetId}", mediaAsset.Id);
            return cancelUploadResult.Error.ToErrors();
        }

        var deleteMediaAssetResult = await _mediaAssetRepository
            .RemoveAsync(mediaAsset, cancellationToken);

        if (deleteMediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Can`t delete the media asset {mediaAssetId}", mediaAsset.Id);
            return deleteMediaAssetResult.Error.ToErrors();
        }

        return Result.Success<Errors>();
    }
}
