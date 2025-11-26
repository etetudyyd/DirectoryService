using DevQuestions.Domain.Entities;
using DirectoryService.Contracts.Locations.Dtos;

namespace DirectoryService.Contracts.Locations.Requests;

public record CreateLocationRequest(
    NameDto Name,
    AddressDto Address,
    TimezoneDto Timezone,
    Guid[] DepartmentsIds);