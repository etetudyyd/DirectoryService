namespace DirectoryService.Requests;

public record StartMultipartUploadFileRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long Size,
    string Context,
    Guid ContextId);