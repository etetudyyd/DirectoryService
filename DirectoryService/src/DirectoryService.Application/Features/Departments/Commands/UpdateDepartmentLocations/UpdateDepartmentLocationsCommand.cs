﻿using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.Features.Departments.Commands.UpdateDepartmentLocations;

public record UpdateDepartmentLocationsCommand(Guid DepartmentId, UpdateDepartmentLocationsRequest DepartmentRequest) : ICommand;