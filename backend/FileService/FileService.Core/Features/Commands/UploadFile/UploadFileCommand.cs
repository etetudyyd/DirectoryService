using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.UploadFile;

public record UploadFileCommand(UploadFileRequest Request) : ICommand;