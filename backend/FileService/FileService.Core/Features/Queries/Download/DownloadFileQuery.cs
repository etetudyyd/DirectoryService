using Core.Abstractions;

namespace DirectoryService.Features.Queries.Download;

public record DownloadFileQuery(Guid FileId) : IQuery;