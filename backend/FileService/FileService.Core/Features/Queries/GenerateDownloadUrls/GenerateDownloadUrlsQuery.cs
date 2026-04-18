using Core.Abstractions;

namespace DirectoryService.Features.Queries.GenerateDownloadUrls;

public record GenerateDownloadUrlsQuery(string[] Paths) : IQuery;