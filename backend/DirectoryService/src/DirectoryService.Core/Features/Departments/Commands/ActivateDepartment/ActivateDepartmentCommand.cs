using Core.Abstractions;

namespace DirectoryService.Features.Departments.Commands.ActivateDepartment;

public record ActivateDepartmentCommand(Guid Id): ICommand;