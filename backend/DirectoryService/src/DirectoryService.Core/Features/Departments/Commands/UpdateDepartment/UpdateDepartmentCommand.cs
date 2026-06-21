using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(Guid DepartmentId, UpdateDepartmentRequest Request) : ICommand;