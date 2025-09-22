using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments.CreateDepartment;

public class CreateDepartmentCommand(CreateDepartmentDto DepartmentDto) : ICommand;