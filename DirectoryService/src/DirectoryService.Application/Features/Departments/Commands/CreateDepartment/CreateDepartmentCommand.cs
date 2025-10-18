using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest DepartmentRequest) : ICommand;