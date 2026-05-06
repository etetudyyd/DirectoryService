using DirectoryService.Dtos;

namespace DirectoryService.Responses;

public record GetMediaAssetsInfoResponse(IReadOnlyList<MediaAssetsInfoDto> MediaAssets);