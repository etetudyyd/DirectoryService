using Core.Abstractions;

namespace DirectoryService.Features.DownloadFile;

public record DownloadFileQuery(Guid FileId) : IQuery;