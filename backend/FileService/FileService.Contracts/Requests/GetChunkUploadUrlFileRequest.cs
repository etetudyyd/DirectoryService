namespace DirectoryService.Requests;

public record GetChunkUploadUrlFileRequest(
    Guid MediaAssetId,
    string UploadId,
    int PartNumber);