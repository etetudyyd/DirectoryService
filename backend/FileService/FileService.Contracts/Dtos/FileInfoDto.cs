namespace DirectoryService.Dtos;

public record FileInfoDto(
    string FileName,
    string ContentType,
    long Size);