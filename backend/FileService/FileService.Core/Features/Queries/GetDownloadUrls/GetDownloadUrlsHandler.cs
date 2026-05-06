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

namespace DirectoryService.Features.Queries.GetDownloadUrls;

public class GenerateDownloadUrlsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/download/urls", async Task<EndpointResult<string[]>>(
            [FromQuery] string[] paths,
            [FromServices] GetDownloadUrlsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDownloadUrlsQuery(paths);
            return await handler.Handle(query, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class GetDownloadUrlsHandler : IQueryHandler<string[], GetDownloadUrlsQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<GetDownloadUrlsQuery> _validator;
    private readonly ILogger<GetDownloadUrlsHandler> _logger;

    public GetDownloadUrlsHandler(
        IS3Provider s3Provider,
        ILogger<GetDownloadUrlsHandler> logger,
        IValidator<GetDownloadUrlsQuery> validator)
    {
        _s3Provider = s3Provider;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string[], Errors>> Handle(GetDownloadUrlsQuery query, CancellationToken cancellationToken)
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

        var downloadResult = await _s3Provider.GetDownloadUrlsAsync(storageKeys);

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