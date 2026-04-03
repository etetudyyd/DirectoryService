using Core.Abstractions;

namespace DirectoryService.Features.Download;

public record DownloadFileQuery(Guid FileId) : IQuery;