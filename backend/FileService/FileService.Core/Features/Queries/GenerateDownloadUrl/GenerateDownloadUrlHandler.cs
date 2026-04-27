using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using DirectoryService.VOs;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GenerateDownloadUrl;

public class GenerateDownloadUrlEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/download/url/{*path}", async Task<EndpointResult<string>>(
            [FromRoute] string path,
            [FromServices] GenerateDownloadUrlHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GenerateDownloadUrlQuery(path);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GenerateDownloadUrlHandler : IQueryHandler<string, GenerateDownloadUrlQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<GenerateDownloadUrlQuery> _validator;
    private readonly ILogger<GenerateDownloadUrlHandler> _logger;

    public GenerateDownloadUrlHandler(
        IS3Provider s3Provider,
        ILogger<GenerateDownloadUrlHandler> logger,
        IValidator<GenerateDownloadUrlQuery> validator)
    {
        _s3Provider = s3Provider;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string, Errors>> Handle(GenerateDownloadUrlQuery query, CancellationToken cancellationToken)
    {
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {query}", query);
            return validatorResult.ToErrors();
        }

        string path = query.Path;

        (string bucket, string? prefix, string key) = StorageKeyParser.TryParse(path);

        var storageKey = StorageKey.Create(bucket, prefix, key).Value;

        var downloadResult = await _s3Provider.GenerateDownloadUrlAsync(storageKey);

        if (downloadResult.IsFailure)
        {
            _logger.LogInformation("Failed generation to download file url with id:{key}", key);
            return downloadResult.Error.ToErrors();
        }

        _logger.LogInformation("Download file url was generated with id:{key}", key);

        return downloadResult.Value.PresignedUrl;
    }
}