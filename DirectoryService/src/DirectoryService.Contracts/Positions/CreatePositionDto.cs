using DevQuestions.Domain.Entities;

namespace DirectoryService.Contracts.Positions;

public record CreatePositionDto(
    NameDto Name,
    DescriptionDto Description,
    IEnumerable<DepartmentPosition> DepartmentPositions);