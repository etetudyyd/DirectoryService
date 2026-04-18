using Core.Abstractions;

namespace DirectoryService.Features.Commands.DeleteFile;

public record DeleteFileCommand(Guid FileId) : ICommand;