using DirectoryService.Application.Abstractions.Commands;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid Id) : ICommand;