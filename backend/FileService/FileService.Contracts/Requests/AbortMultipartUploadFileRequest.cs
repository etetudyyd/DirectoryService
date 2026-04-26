namespace DirectoryService.Requests;

public record AbortMultipartUploadFileRequest(
    Guid MediaAssetId,
    string UploadId);