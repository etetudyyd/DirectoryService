using DevQuestions.Domain.Entities;

namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    IEnumerable<DepartmentLocation> DepartmentLocations);