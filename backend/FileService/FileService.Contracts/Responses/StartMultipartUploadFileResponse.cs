namespace DirectoryService.Responses;

public record StartMultipartUploadFileResponse(
    Guid MediaAssetId,
    string UploadId,
    IEnumerable<string> UploadUrls,
    long Size
    );