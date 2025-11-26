using DevQuestions.Domain.Entities;
using DirectoryService.Contracts.Positions.VODtos;

namespace DirectoryService.Contracts.Positions.Requests;

public record CreatePositionRequest(
    string Name,
    string Description,
    Guid[] DepartmentsIds);