using Core.Abstractions;
using CSharpFunctionalExtensions;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.GenerateUploadUrl;


public class GenerateUploadUrlEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/files/upload/url/{id:guid}", async Task<EndpointResult<string>>(
            [FromRoute] Guid id,
            [FromServices] GenerateUploadUrlHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GenerateUploadUrlCommand(id);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GenerateUploadUrlHandler : ICommandHandler<string, GenerateUploadUrlCommand>
{
    private readonly IMediaAssetsRepository _mediaAssetRepository;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<GenerateUploadUrlHandler> _logger;

    public GenerateUploadUrlHandler(
        IMediaAssetsRepository mediaAssetRepository,
        IS3Provider s3Provider,
        ILogger<GenerateUploadUrlHandler> logger)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public async Task<Result<string, Errors>> Handle(GenerateUploadUrlCommand command, CancellationToken cancellationToken)
    {
        var fileId = command.FileId;
        var mediaAssetResult = await _mediaAssetRepository
            .GetBy(x => x.Id == fileId, cancellationToken);

        if (mediaAssetResult.IsFailure)
        {
            _logger.LogInformation("Failed to find file with id:{fileId}", fileId);
            return mediaAssetResult.Error.ToErrors();
        }

        var mediaAsset = mediaAssetResult.Value;

        var downloadResult = await _s3Provider.GenerateUploadUrlAsync(mediaAsset.RawKey, mediaAsset.MediaData, cancellationToken);

        if (downloadResult.IsFailure)
        {
            _logger.LogInformation("Failed upload file url with id:{fileId}", fileId);
            return downloadResult.Error.ToErrors();
        }

        _logger.LogInformation("Upload file url with id:{fileId}", fileId);

        return downloadResult.Value;
    }
}