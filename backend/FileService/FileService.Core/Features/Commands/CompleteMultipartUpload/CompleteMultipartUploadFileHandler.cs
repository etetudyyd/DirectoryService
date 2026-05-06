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

namespace DirectoryService.Features.Commands.CompleteMultipartUpload;

public class CompleteMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart/complete", async Task<EndpointResult>(
            [FromBody] CompleteMultipartUploadFileRequest request,
            [FromServices] CompleteMultipartUploadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CompleteMultipartUploadFileCommand(request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class CompleteMultipartUploadFileHandler : ICommandHandler<CompleteMultipartUploadFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<CompleteMultipartUploadFileHandler> _logger;
    private readonly IValidator<CompleteMultipartUploadFileCommand> _validator;

    public CompleteMultipartUploadFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<CompleteMultipartUploadFileHandler> logger,
        IValidator<CompleteMultipartUploadFileCommand> validator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Errors>> Handle(
        CompleteMultipartUploadFileCommand command,
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

        if (mediaAsset.MediaData.ExpectedChunksCount != request.PartETags.Count)
        {
            return GeneralErrors.General.ValueIsInvalid("Amount of eTags is not equal to amount of chunks.").ToErrors();
        }

        var completeResult = await _s3Provider.CompleteMultipartUploadAsync(
            mediaAsset.RawKey,
            request.UploadId,
            request.PartETags,
            cancellationToken);

        if (completeResult.IsFailure)
        {
            _logger.LogInformation("Can`t complete uploading the media asset {mediaAssetId}", request.MediaAssetId);
            return completeResult.Error.ToErrors();
        }

        mediaAsset.MarkUploaded(DateTime.UtcNow);

        await _mediaAssetRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Completed uploading the media asset {mediaAssetId}", request.MediaAssetId);

        return UnitResult.Success<Errors>();
    }
}