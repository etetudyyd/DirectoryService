using Core.Abstractions;

namespace DirectoryService.Features.Commands.GenerateUploadUrl;

public record GenerateUploadUrlCommand(Guid FileId) : ICommand;