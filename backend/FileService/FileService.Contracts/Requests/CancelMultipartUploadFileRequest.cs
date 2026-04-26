namespace DirectoryService.Requests;

public record CancelMultipartUploadFileRequest(
    Guid MediaAssetId,
    string UploadId);