using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    private readonly ILogger<CreateDepartmentHandler> _logger;

    private const string DEPARTMENT_TABLE_ROUTE = "\"DirectoryService\".\"departments\"";


    public DepartmentsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<CreateDepartmentHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return department.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding department");
            return Error.Failure("ErrorType.FAILURE", "Department not found");
        }
    }

    public async Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Include(d => d.DepartmentLocations)
            .FirstOrDefaultAsync(d => d.Id == new DepartmentId(id), cancellationToken);

        if (department is null)
        {
            return Error.Failure("department.not.found", "Department not found");
        }

        return Result.Success<Department, Error>(department);
    }

    public async Task<bool> IsIdentifierExistAsync(string identifier, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AnyAsync(
                d => d.Identifier.Value == identifier,
                cancellationToken: cancellationToken);
    }

    public async Task<bool> AllDepartmentLocationsExistsAndActiveAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        int count = await _dbContext.Departments
            .Where(d => ids.Contains(d.Id.Value))
            .Where(d => d.IsActive)
            .CountAsync(cancellationToken);

        return count == ids.Count;
    }

    public async Task<Result<Department, Error>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .FromSqlRaw(
                $@"SELECT * 
                   FROM {DEPARTMENT_TABLE_ROUTE} 
                   WHERE id = {{0}} 
                   FOR UPDATE", id)
            .FirstOrDefaultAsync(cancellationToken);

        if (department == null)
            return Error.Failure("department.not.found", "Department not found");

        return department;
    }

    public async Task<UnitResult<Error>> LockDescendantsAsync(Department department, CancellationToken cancellationToken)
    {
        var conn = _dbContext.Database.GetDbConnection();

        const string sql =
            $$"""
              SELECT id 
              FROM {{DEPARTMENT_TABLE_ROUTE}} 
              WHERE path <@ @rootPath::ltree
              AND id != @selfId
              FOR UPDATE
              """;

        await conn.ExecuteAsync(sql, new
        {
            rootPath = department.Path.Value,
            selfId = department.Id.Value,
        });

        return UnitResult.Success<Error>();
    }

    public async Task<bool> IsDescendantAsync(Path rootPath, Guid candidateForCheckId, CancellationToken cancellationToken)
    {
        var conn = _dbContext.Database.GetDbConnection();

        const string dapperSql =
            $$"""
              SELECT 1 
              FROM {{DEPARTMENT_TABLE_ROUTE}} 
              WHERE path <@ @rootPath::ltree
              AND id = @candidateId
              """;

        var result = await conn.QueryFirstOrDefaultAsync<int?>(dapperSql, new
        {
            rootPath = rootPath.Value,
            candidateId = candidateForCheckId,
        });

        return result.HasValue;
    }

    public async Task<Result<Guid, Error>> RelocateDepartmentAsync(
        Department departmentUpdated,
        Path oldPath, CancellationToken cancellationToken)
    {
        var conn = _dbContext.Database.GetDbConnection();

        const string dapperSql =
            $$"""
              UPDATE {{DEPARTMENT_TABLE_ROUTE}}
              SET
                  parent_id = CASE 
                                  WHEN id = @id THEN @parentId 
                                  ELSE parent_id 
                              END,
                  path = CASE 
                              WHEN id = @id THEN @newPath::ltree
                              ELSE @newPath::ltree || subpath(path, nlevel(@oldPath::ltree))
                         END,
                  depth = CASE
                              WHEN id = @id THEN @newDepth
                              ELSE @newDepth + (depth - nlevel(@oldPath::ltree))
                          END
              WHERE path <@ @oldPath::ltree
              """;

        await conn.ExecuteAsync(dapperSql, new
        {
            id = departmentUpdated.Id.Value,
            parentId = departmentUpdated.ParentId.Value,
            newPath = departmentUpdated.Path.Value,
            oldPath = oldPath.Value,
            newDepth = departmentUpdated.Depth,
        });

        return Result.Success<Guid, Error>(departmentUpdated.Id.Value);
    }

}