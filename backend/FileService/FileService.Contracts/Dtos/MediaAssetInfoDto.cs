namespace DirectoryService;

public record MediaAssetInfoDto(
    Guid MediaAssetId,
    string Status,
    string AssetType,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    FileInfoDto FileInfo,
    string? DownloadUrl);