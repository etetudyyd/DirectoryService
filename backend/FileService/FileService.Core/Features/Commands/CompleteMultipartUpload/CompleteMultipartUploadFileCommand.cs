using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.CompleteMultipartUpload;

public record CompleteMultipartUploadFileCommand(CompleteMultipartUploadFileRequest Request) : ICommand;