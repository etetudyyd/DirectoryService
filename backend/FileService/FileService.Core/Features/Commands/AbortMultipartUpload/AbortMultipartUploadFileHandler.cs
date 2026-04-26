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

namespace DirectoryService.Features.Commands.AbortMultipartUpload;

public class AbortMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart/abort", async Task<EndpointResult>(
            [FromBody] AbortMultipartUploadFileRequest request,
            [FromServices] AbortMultipartUploadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AbortMultipartUploadFileCommand(request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class AbortMultipartUploadFileHandler : ICommandHandler<AbortMultipartUploadFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<AbortMultipartUploadFileHandler> _logger;
    private readonly IValidator<AbortMultipartUploadFileCommand> _validator;

    public AbortMultipartUploadFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<AbortMultipartUploadFileHandler> logger,
        IValidator<AbortMultipartUploadFileCommand> validator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Errors>> Handle(
        AbortMultipartUploadFileCommand command,
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

        var abortResult = await _s3Provider
            .AbortMultipartUploadAsync(mediaAsset.RawKey, request.UploadId, cancellationToken);

        if (abortResult.IsFailure)
        {
            _logger.LogInformation("Can`t abort uploading the media asset {mediaAssetId}", request.MediaAssetId);
            return abortResult.Error.ToErrors();
        }

        mediaAsset.MarkFailed(DateTime.UtcNow);

        await _mediaAssetRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Aborted uploading the media asset {mediaAssetId}", request.MediaAssetId);

        return Result.Success<Errors>();
    }
}