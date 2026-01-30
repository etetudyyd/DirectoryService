namespace DirectoryService.Positions.Requests;

public record UpdatePositionRequest(
    string Name,
    string Description,
    Guid[] DepartmentsIds);