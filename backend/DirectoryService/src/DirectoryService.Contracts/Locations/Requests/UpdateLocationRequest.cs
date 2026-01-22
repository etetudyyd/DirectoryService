using DirectoryService.Locations.Dtos;

namespace DirectoryService.Locations.Requests;

public record UpdateLocationRequest(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    Guid[] DepartmentsIds
    );