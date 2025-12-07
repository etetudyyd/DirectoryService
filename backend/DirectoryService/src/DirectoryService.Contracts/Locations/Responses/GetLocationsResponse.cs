using DirectoryService.Locations.Dtos;

namespace DirectoryService.Locations.Responses;

public record GetLocationsResponse(List<LocationDto> Locations, long TotalCount);


