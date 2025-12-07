using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest DepartmentRequest) : ICommand;