﻿namespace DirectoryService.Contracts.Departments.Responses;

public record GetTopDepartmentsByPositionsResponse(IEnumerable<DepartmentResponse> Departments);