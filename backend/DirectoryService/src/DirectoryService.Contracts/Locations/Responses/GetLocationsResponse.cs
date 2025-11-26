using DirectoryService.Contracts.Locations.Dtos;
using DirectoryService.Contracts.Locations.Requests;

namespace DirectoryService.Contracts.Locations.Responses;

public record GetLocationsResponse(List<LocationDto> Locations, long TotalCount);


