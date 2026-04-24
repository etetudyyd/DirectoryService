namespace DirectoryService.Responses;

public record GetChunkUploadUrlFileResponse(
    string UploadId,
    int PartNumber);