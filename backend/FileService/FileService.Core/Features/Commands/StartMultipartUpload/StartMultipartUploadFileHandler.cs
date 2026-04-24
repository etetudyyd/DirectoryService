using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Requests;
using DirectoryService.Responses;
using DirectoryService.Types;
using DirectoryService.VOs;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.StartMultipartUpload;

public class StartMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart/start", async Task<EndpointResult<StartMultipartUploadFileResponse>>(
            [FromBody] StartMultipartUploadFileRequest request,
            [FromServices] StartMultipartUploadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new StartMultipartUploadFileCommand(request);
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class StartMultipartUploadFileHandler : ICommandHandler<StartMultipartUploadFileResponse, StartMultipartUploadFileCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly IChunkSizeCalculator _chunkSizeCalculator;
    private readonly ILogger<StartMultipartUploadFileHandler> _logger;
    private readonly IValidator<StartMultipartUploadFileCommand> _validator;

    public StartMultipartUploadFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<StartMultipartUploadFileHandler> logger,
        IValidator<StartMultipartUploadFileCommand> validator,
        IChunkSizeCalculator chunkSizeCalculator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
        _chunkSizeCalculator = chunkSizeCalculator;
    }

    public async Task<Result<StartMultipartUploadFileResponse, Errors>> Handle(StartMultipartUploadFileCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {command}", command);
            return validationResult.ToErrors();
        }

        var request = command.Request;

        var chunkSizeResult = _chunkSizeCalculator
            .Calculate(request.Size);

        if (chunkSizeResult.IsFailure)
        {
            _logger.LogInformation("Invalid chunk size: {chunkSizeResult}", chunkSizeResult.Error);
            return chunkSizeResult.Error.ToErrors();
        }

        var mediaDataResult = MediaData
            .Create(
                FileName.Create(request.FileName).Value,
                ContentType.Create(request.ContentType).Value,
                chunkSizeResult.Value.ChunkSize,
                chunkSizeResult.Value.TotalChunks);

        if (mediaDataResult.IsFailure)
        {
            _logger.LogInformation("Failed to create media data: {mediaDataResult}", mediaDataResult.Error);
            return mediaDataResult.Error.ToErrors();
        }

        var mediaOwnerResult = MediaOwner
            .Create(request.Context, request.ContextId);

        if (mediaOwnerResult.IsFailure)
        {
            _logger.LogInformation("Failed to create media owner: {mediaOwnerResult}", mediaOwnerResult.Error);
            return mediaOwnerResult.Error.ToErrors();
        }

        var mediaAssetResult = MediaAsset
            .CreateForUpload(mediaDataResult.Value, request.AssetType.ToAssetType(), mediaOwnerResult.Value);

        if (mediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Failed to create media asset: {mediaAssetResult}", mediaAssetResult.Error);
            return mediaAssetResult.Error.ToErrors();
        }

        await _mediaAssetRepository.AddAsync(mediaAssetResult.Value, cancellationToken);

        var startMultipartUploadResult = await _s3Provider
            .StartMultipartUploadAsync(mediaAssetResult.Value.RawKey, mediaAssetResult.Value.MediaData, cancellationToken);

        if (startMultipartUploadResult.IsFailure)
        {
            _logger.LogInformation("Failed to start multipart upload: {multipartUploadResult}", startMultipartUploadResult.Error);
            return startMultipartUploadResult.Error.ToErrors();
        }

        var generateChunkUploadUrlsResult = await _s3Provider
            .GenerateAllChunkUploadUrls(
                mediaAssetResult.Value.RawKey,
                startMultipartUploadResult.Value,
                chunkSizeResult.Value.TotalChunks,
                cancellationToken);

        if (generateChunkUploadUrlsResult.IsFailure)
        {
            _logger.LogInformation("Failed to generate chunk upload URLs: {generateChunkUploadUrlsResult}", generateChunkUploadUrlsResult.Error);
            return generateChunkUploadUrlsResult.Error.ToErrors();
        }

        await _mediaAssetRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully generated media asset: {mediaAsset}", mediaAssetResult.Value);

        return new StartMultipartUploadFileResponse(
            mediaAssetResult.Value.Id,
            startMultipartUploadResult.Value,
            generateChunkUploadUrlsResult.Value,
            chunkSizeResult.Value.ChunkSize);
    }
}