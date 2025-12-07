using Core.Abstractions;

namespace DirectoryService.Features.Locations.Queries.GetLocationById;

public record GetLocationByIdQuery(Guid Id) : IQuery;