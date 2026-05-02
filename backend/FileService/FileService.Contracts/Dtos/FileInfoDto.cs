namespace DirectoryService;

public record FileInfoDto(
    string FileName,
    string ContentType,
    long Size);