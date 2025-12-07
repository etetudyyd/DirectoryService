namespace DirectoryService.Positions.Requests;

public record CreatePositionRequest(
    string Name,
    string Description,
    Guid[] DepartmentsIds);