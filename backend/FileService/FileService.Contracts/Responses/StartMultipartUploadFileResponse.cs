namespace DirectoryService.Responses;

public record StartMultipartUploadFileResponse(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<ChunkUploadUrl> ChunkUploadUrls,
    long ChunkSize
    );