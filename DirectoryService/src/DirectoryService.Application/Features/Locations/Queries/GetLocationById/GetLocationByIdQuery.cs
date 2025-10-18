using DirectoryService.Application.Abstractions.Queries;

namespace DirectoryService.Application.Features.Locations.Queries.GetLocationById;

public record GetLocationByIdQuery(Guid Id) : IQuery;