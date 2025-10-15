using DevQuestions.Domain.Entities;
using DirectoryService.Contracts.Positions.VODtos;

namespace DirectoryService.Contracts.Positions.Requests;

public record CreatePositionRequest(
    NameDto Name,
    DescriptionDto Description,
    IEnumerable<DepartmentPosition> DepartmentPositions);