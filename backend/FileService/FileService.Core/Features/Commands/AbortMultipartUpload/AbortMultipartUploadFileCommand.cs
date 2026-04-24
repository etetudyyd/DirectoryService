using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.AbortMultipartUpload;

public record AbortMultipartUploadFileCommand(AbortMultipartUploadFileRequest Request) : ICommand;