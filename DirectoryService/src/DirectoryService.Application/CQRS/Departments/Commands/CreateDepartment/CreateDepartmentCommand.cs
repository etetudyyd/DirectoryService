using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.CQRS.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest DepartmentRequest) : ICommand;