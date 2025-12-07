using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Commands.RelocateDepartmentParent;

public record RelocateDepartmentParentCommand(Guid DepartmentId, RelocateDepartmentParentRequest DepartmentRequest) : ICommand;