using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.DepartmentVO;

namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentDto(
    string Name,
    string Identifier,
    Guid? ParentId,
    Guid[] LocationsIds);