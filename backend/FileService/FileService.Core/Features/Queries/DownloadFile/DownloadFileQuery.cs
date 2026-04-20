using Core.Abstractions;

namespace DirectoryService.Features.Queries.DownloadFile;

public record DownloadFileQuery(Guid FileId) : IQuery;