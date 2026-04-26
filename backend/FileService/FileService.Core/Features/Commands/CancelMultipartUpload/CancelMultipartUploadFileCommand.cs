using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.CancelMultipartUpload;

public record CancelMultipartUploadFileCommand(CancelMultipartUploadFileRequest Request) : ICommand;