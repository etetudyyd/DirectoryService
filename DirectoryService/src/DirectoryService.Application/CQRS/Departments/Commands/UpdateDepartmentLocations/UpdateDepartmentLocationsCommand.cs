using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.CQRS.Departments.Commands.UpdateDepartmentLocations;

public record UpdateDepartmentLocationsCommand(Guid DepartmentId, UpdateDepartmentLocationsRequest DepartmentRequest) : ICommand;