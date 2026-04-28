namespace DirectoryService.Responses;

public record GetMediaAssetsInfoResponse(IReadOnlyList<MediaAssetsInfoDto> MediaAssets);