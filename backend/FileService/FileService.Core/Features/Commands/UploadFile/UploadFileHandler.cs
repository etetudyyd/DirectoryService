using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.Requests;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.UploadFile;

public sealed class UploadFileEndpoint : IEndpoint
{
     public void MapEndpoint(IEndpointRouteBuilder app)
     {
         app.MapPost("/files/upload", async Task<EndpointResult<Guid>>(
             [FromForm] UploadFileRequest request,
             [FromServices] UploadFileHandler handler,
             CancellationToken cancellationToken) =>
         {
             var command = new UploadFileCommand(request);
             return await handler.Handle(command, cancellationToken);
         }).DisableAntiforgery();
     }
}

public class UploadFileHandler : ICommandHandler<Guid, UploadFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<UploadFileCommand> _validator;
    private readonly ILogger<UploadFileHandler> _logger;

    public UploadFileHandler(
        IMediaAssetsRepository mediaAssetRepository,
        IS3Provider s3Provider,
        IValidator<UploadFileCommand> validator,
        ILogger<UploadFileHandler> logger)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _s3Provider = s3Provider;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UploadFileCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {command}", command);
            return validationResult.ToErrors();
        }

        var request = command.Request;

        var mediaDataResult = MediaData.Create(
            FileName.Create(request.FormFile.FileName).Value,
            ContentType.Create(request.FormFile.ContentType).Value,
            request.FormFile.Length,
            1);

        if (mediaDataResult.IsFailure)
            return mediaDataResult.Error.ToErrors();

        var mediaData = mediaDataResult.Value;
        var owner = MediaOwner.Create(request.Context, request.ContextId);

        var mediaAssetResult = MediaAsset.CreateForUpload(mediaData, request.AssetType.ToAssetType(), owner.Value);

        if (mediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Failed to create file {fileName}", request.FormFile.Name);
            return mediaAssetResult.Error.ToErrors();
        }

        var mediaAsset = mediaAssetResult.Value;
        var addResult = await _mediaAssetRepository.AddAsync(mediaAsset, cancellationToken);
        if (addResult.IsFailure)
        {
            _logger.LogInformation("Failed to upload file {fileName}", request.FormFile.Name);
            return addResult.Error.ToErrors();
        }

        var uploadResult = await _s3Provider.UploadFileAsync(
            mediaAsset.RawKey,
            request.FormFile.OpenReadStream(),
            mediaData,
            cancellationToken);
        if (uploadResult.IsFailure)
        {
            mediaAsset.MarkFailed(DateTime.UtcNow);
            await _mediaAssetRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Failed to upload file {fileName}", request.FormFile.Name);
            return uploadResult.Error.ToErrors();
        }

        mediaAsset.MarkUploaded(DateTime.UtcNow);

        await _mediaAssetRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Uploaded file {request.FormFile.Name}");

        return addResult.Value;
    }
}