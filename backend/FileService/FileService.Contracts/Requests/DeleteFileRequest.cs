using Core.Abstractions;

namespace DirectoryService.Requests;

public record DeleteFileRequest(Guid FileId) : ICommand;