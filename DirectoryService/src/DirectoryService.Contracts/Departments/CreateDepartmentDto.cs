using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.DepartmentVO;

namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentDto(
    NameDto Name,
    IdentifierDto Identifier,
    DepartmentId? ParentId,
    IEnumerable<DepartmentLocation> DepartmentLocations);