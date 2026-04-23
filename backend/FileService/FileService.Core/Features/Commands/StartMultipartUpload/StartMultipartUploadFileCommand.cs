using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.StartMultipartUpload;

public record StartMultipartUploadFileCommand(StartMultipartUploadFileRequest Request) : ICommand;