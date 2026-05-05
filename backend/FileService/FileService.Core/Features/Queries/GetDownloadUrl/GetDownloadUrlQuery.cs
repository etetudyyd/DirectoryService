using Core.Abstractions;

namespace DirectoryService.Features.Queries.GetDownloadUrl;

public record GetDownloadUrlQuery(string Path) : IQuery;