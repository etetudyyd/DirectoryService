using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.Upload;

public record UploadFileCommand(UploadFileRequest Request) : ICommand;