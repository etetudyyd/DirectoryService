namespace DirectoryService.Responses;

public record GetChunkUploadUrlFileResponse(
    string UploadUrl,
    int PartNumber);