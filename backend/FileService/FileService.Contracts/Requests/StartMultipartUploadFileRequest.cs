namespace DirectoryService.Features.Commands.StartMultipartUploadFile;

public record StartMultipartUploadFileRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long Size,
    string Context,
    Guid ContextId);