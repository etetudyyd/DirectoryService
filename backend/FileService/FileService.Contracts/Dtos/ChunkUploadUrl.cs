namespace DirectoryService.Dtos;

public record ChunkUploadUrl(
    int PartNumber,
    string UploadUrl);