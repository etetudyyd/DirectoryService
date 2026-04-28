using System.Collections.Immutable;
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

namespace DirectoryService.Features.Queries.GenerateDownloadUrls;

public class GenerateDownloadUrlsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/download/urls", async Task<EndpointResult<string[]>>(
            [FromQuery] string[] paths,
            [FromServices] GenerateDownloadUrlsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GenerateDownloadUrlsQuery(paths);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GenerateDownloadUrlsHandler : IQueryHandler<string[], GenerateDownloadUrlsQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<GenerateDownloadUrlsQuery> _validator;
    private readonly ILogger<GenerateDownloadUrlsHandler> _logger;

    public GenerateDownloadUrlsHandler(
        IS3Provider s3Provider,
        ILogger<GenerateDownloadUrlsHandler> logger,
        IValidator<GenerateDownloadUrlsQuery> validator)
    {
        _s3Provider = s3Provider;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string[], Errors>> Handle(GenerateDownloadUrlsQuery query, CancellationToken cancellationToken)
    {
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            _logger.LogInformation("Invalid request: {query}", query);
            return validatorResult.ToErrors();
        }

        List<StorageKey> storageKeys = [];

        foreach (string path in query.Paths)
        {
            (string bucket, string? prefix, string key) = StorageKeyParser.TryParse(path);

            var storageKey = StorageKey.Create(bucket, prefix, key).Value;

            storageKeys.Add(storageKey);
        }

        var downloadResult = await _s3Provider.GenerateDownloadUrlsAsync(storageKeys);

        if (downloadResult.IsFailure)
        {
            _logger.LogInformation("Failed generation to download file urls with ids:{downloadResult.Value}", string.Join(", ", downloadResult.Value));
            return downloadResult.Error;
        }

        string[] urls = downloadResult.Value.Select(m => m.PresignedUrl).ToArray();

        _logger.LogInformation("Download file urls was generated with ids:{downloadResult.Value}", string.Join(", ", downloadResult.Value));

        return urls;
    }
}