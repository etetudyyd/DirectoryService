﻿namespace DirectoryService.Contracts.Departments.Responses;

public record GetRootDepartmentsResponse(IEnumerable<DepartmentPrefetchResponse> Departments);