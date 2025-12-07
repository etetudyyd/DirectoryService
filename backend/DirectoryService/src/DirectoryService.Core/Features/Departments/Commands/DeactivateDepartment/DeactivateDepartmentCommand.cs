using Core.Abstractions;

namespace DirectoryService.Features.Departments.Commands.DeactivateDepartment;

public record DeactivateDepartmentCommand(Guid Id) : ICommand;