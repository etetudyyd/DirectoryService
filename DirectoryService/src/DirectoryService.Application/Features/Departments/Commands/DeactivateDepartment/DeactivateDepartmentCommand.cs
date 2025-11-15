using DirectoryService.Application.Abstractions.Commands;

namespace DirectoryService.Application.Features.Departments.Commands.DeactivateDepartment;

public record DeactivateDepartmentCommand(Guid Id) : ICommand;