using DevQuestions.Domain.Entities;

namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    bool IsActive,
    IEnumerable<DepartmentLocation> DepartmentLocations);