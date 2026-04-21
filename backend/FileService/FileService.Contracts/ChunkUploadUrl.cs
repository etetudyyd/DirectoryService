namespace DirectoryService;

public record ChunkUploadUrl(
    int PartNumber,
    string UploadUrl);