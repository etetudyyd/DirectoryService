using Core.Abstractions;

namespace DirectoryService.Features.Queries.GenerateDownloadUrl;

public record GenerateDownloadUrlQuery(string Path) : IQuery;