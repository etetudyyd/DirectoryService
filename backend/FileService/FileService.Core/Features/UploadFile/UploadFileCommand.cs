using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.UploadFile;

public record UploadFileCommand(UploadFileRequest Request) : ICommand;