namespace DirectoryService.Requests;

public record GetMediaAssetsInfoRequest(IReadOnlyList<Guid> MediaAssetIds);