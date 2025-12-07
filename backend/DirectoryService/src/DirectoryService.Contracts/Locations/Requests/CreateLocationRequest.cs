using DirectoryService.Locations.Dtos;

namespace DirectoryService.Locations.Requests;

public record CreateLocationRequest(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    Guid[] DepartmentsIds);