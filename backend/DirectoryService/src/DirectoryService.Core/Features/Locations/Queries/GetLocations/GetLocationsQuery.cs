using Core.Abstractions;
using DirectoryService.Locations.Requests;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

public record GetLocationsQuery(GetLocationsRequest LocationsRequest) : IQuery;