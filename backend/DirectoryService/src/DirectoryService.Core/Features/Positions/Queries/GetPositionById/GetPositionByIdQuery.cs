using Core.Abstractions;

namespace DirectoryService.Features.Positions.Queries.GetPositionById;

public record GetPositionByIdQuery(Guid Id) : IQuery;