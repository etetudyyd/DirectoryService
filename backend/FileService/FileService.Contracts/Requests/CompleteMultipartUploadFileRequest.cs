namespace DirectoryService.Requests;

public record CompleteMultipartUploadFileRequest(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<PartETagDto> PartETags);