using Core.Abstractions;
using DirectoryService.Requests;

namespace DirectoryService.Features.Queries.GetMediaAssetsInfo;

public record GetMediaAssetsInfoHandlerQuery(GetMediaAssetsInfoRequest Request) : IQuery;