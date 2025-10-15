using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Contracts.Locations.Requests;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocationById;

public record GetLocationByIdQuery(Guid Id) : IQuery;