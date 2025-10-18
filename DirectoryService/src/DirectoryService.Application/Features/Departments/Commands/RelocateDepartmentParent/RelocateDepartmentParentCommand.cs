using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.Features.Departments.Commands.RelocateDepartmentParent;

public record RelocateDepartmentParentCommand(Guid DepartmentId, RelocateDepartmentParentRequest DepartmentRequest) : ICommand;