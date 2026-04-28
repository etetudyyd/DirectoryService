using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Commands.GetChunkUploadUrl;

public record GetChunkUploadUrlFileCommand(GetChunkUploadUrlFileRequest Request) : ICommand;