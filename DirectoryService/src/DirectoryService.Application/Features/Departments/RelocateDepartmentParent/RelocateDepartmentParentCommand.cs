using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments.RelocateDepartmentParent;

public record RelocateDepartmentParentCommand(Guid DepartmentId, RelocateDepartmentParentDto DepartmentDto) : ICommand;