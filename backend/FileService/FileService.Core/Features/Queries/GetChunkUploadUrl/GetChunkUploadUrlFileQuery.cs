using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Queries.GetChunkUploadUrl;

public record GetChunkUploadUrlFileQuery(GetChunkUploadUrlFileRequest Request) : IQuery;