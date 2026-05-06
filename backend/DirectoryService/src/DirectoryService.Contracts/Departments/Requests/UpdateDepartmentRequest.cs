namespace DirectoryService.Departments.Requests;

public record UpdateDepartmentRequest(
    string Name,
    string Identifier);