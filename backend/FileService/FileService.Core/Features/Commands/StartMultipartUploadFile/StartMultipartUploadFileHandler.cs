using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Requests;
using DirectoryService.Responses;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.StartMultipartUploadFile;

public class StartMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart-upload", async Task<EndpointResult<StartMultipartUploadFileResponse>>(
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
    private readonly ILogger<StartMultipartUploadFileHandler> _logger;
    private readonly IValidator<StartMultipartUploadFileCommand> _validator;

    public StartMultipartUploadFileHandler(
        IS3Provider s3Provider,
        IMediaAssetsRepository mediaAssetRepository,
        ILogger<StartMultipartUploadFileHandler> logger, IValidator<StartMultipartUploadFileCommand> validator)
    {
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<StartMultipartUploadFileResponse, Errors>> Handle(StartMultipartUploadFileCommand command, CancellationToken cancellationToken)
    {
        // выполнить валидацию
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {command}", command);
            return validationResult.ToErrors();
        }
        
        // посчитать кол чанков для загрузки и их размер
        
        // создать домен сущность медиа-ассет, CreateForUpload 
        
        // вызвать StartMultipartUpload
        
        // сгенерировать коллекцию uploadUrl для чанков 
        
        // если успешно, сохранить статус в базу через репозиторий (Uploaded)
        
        // return data MediaAsset(id), uploadId, list of links for the uploading chunks, chunk size

        return new StartMultipartUploadFileResponse();
    }
}