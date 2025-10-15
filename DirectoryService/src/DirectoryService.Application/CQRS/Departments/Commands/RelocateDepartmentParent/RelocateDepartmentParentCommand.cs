using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.CQRS.Departments.Commands.RelocateDepartmentParent;

public record RelocateDepartmentParentCommand(Guid DepartmentId, RelocateDepartmentParentRequest DepartmentRequest) : ICommand;