using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database;
using DirectoryService.Departments.Responses;
using DirectoryService.ValueObjects.Department;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetDepartment;

public class GetDepartmentHandler : IQueryHandler<DepartmentResponse, GetDepartmentQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILogger<GetDepartmentHandler> _logger;
    private readonly HybridCache _cache;

    public GetDepartmentHandler(
        ILogger<GetDepartmentHandler> logger,
        HybridCache cache,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _cache = cache;
        _readDbContext = readDbContext;
    }

    public async Task<Result<DepartmentResponse, Errors>> Handle(GetDepartmentQuery query, CancellationToken cancellationToken)
    {
        var department = await _readDbContext.DepartmentsRead
            .FirstOrDefaultAsync(x => x.Id == new DepartmentId(query.DepartmentId), cancellationToken);
        if (department is null)
        {
            return Error.NotFound("department.not.found", "Department not found")
                .ToErrors();
        }

        _logger.LogInformation("Get department by id: {Id}", department.Id);

        return new DepartmentResponse
        {
            Id = department.Id.Value,
            Name = department.Name.Value,
            Identifier = department.Identifier.Value,
            Path = department.Path.Value,
            ParentId = department.ParentId?.Value,
            Depth = department.Depth,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt,
            DeletedAt = department.DeletedAt,
        };
    }
}