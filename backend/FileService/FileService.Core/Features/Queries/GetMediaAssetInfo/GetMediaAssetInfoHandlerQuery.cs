using Core.Abstractions;

namespace DirectoryService.Features.Queries.GetMediaAssetInfo;

public record GetMediaAssetInfoHandlerQuery(Guid MediaAssetId) : IQuery;