using DevQuestions.Domain.Entities;
using DirectoryService.Contracts.Locations.VODto;

namespace DirectoryService.Contracts.Locations.Requests;

public record CreateLocationRequest(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    IEnumerable<DepartmentLocation> DepartmentLocations);