using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.StartMultipartUploadFile;

public record StartMultipartUploadFileCommand(StartMultipartUploadFileRequest Request) : ICommand;