using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Upload;

public record UploadFileCommand(UploadFileRequest Request) : ICommand;