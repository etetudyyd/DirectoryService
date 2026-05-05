using Core.Abstractions;

namespace DirectoryService.Features.Queries.GetDownloadUrls;

public record GetDownloadUrlsQuery(string[] Paths) : IQuery;