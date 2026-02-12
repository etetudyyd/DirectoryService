using Core.Abstractions;
using DirectoryService.Positions.Requests;

namespace DirectoryService.Features.Positions.Queries.GetPositions;


public record GetPositionsQuery(GetPositionsRequest PositionsRequest) : IQuery;