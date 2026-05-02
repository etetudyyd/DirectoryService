namespace DirectoryService;

public record MediaAssetsInfoDto(
    Guid MediaAssetId,
    string Status,
    string? PresignedUrl);