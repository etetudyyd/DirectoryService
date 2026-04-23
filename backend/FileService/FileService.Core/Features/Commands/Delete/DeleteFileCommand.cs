using Core.Abstractions;

namespace DirectoryService.Features.Commands.Delete;

public record DeleteFileCommand(Guid FileId) : ICommand;