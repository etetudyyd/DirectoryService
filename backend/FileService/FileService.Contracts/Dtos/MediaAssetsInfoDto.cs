namespace DirectoryService.Dtos;

public record MediaAssetsInfoDto(
    Guid MediaAssetId,
    string Status,
    string? PresignedUrl);