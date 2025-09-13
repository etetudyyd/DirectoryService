using DevQuestions.Domain.Entities.AdjacentEntities;

namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    bool IsActive,
    IEnumerable<DepartmentLocation> DepartmentLocations);