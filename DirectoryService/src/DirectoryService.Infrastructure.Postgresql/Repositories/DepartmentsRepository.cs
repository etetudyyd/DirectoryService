using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Features.Departments.Commands.CreateDepartment;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

/// <summary>
/// DepartmentsRepository - realization of Repository Pattern for Departments database logic by using DirectoryServiceDbContext.
/// It realizes interface IDepartmentsRepository.
/// </summary>
public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    private readonly ILogger<CreateDepartmentHandler> _logger;

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
            .FirstOrDefaultAsync(
                d => d.IsActive && d.Id == new DepartmentId(id),
                cancellationToken);

        if (department is null)
        {
            return Error.Failure("department.not.found", "Department not found");
        }

        return department;
    }

    public async Task<Result<Guid, Error>> UpdateChildDepartmentsPath(
        Department parent,
        CancellationToken cancellationToken)
    {
        string oldPath = parent.Path.Value
            .Replace("deleted-", string.Empty);

        const string dapperSql = $"""
                           UPDATE {Constants.DEPARTMENT_TABLE_ROUTE}
                           SET path = REGEXP_REPLACE(path::text, @OldPath || '.*', @NewPath || substring(path::text, length(@OldPath)+1))::ltree
                           WHERE path <@ @OldPath::ltree
                             AND id != @ParentId
                           """;

        var conn = _dbContext.Database.GetDbConnection();

        await conn.ExecuteAsync(dapperSql, new
        {
            OldPath = oldPath,
            NewPath = parent.Path.Value,
            ParentId = parent.Id.Value,
        });

        return parent.Id.Value;
    }

    public async Task<Result<Guid[], Error>> DeactivateConnectedLocations(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        const string dapperSql = $"""
                                      WITH target_locations AS (
                                          SELECT location_id
                                          FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE}
                                          WHERE department_id = @DepartmentId
                                      ),
                                      disabled_locations AS (
                                          SELECT location_id
                                          FROM target_locations tl
                                          WHERE NOT EXISTS (
                                              SELECT 1
                                              FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl
                                              JOIN {Constants.DEPARTMENT_TABLE_ROUTE} d ON d.id = dl.department_id
                                              WHERE dl.location_id = tl.location_id
                                              AND d.is_active = TRUE
                                              AND d.id <> @DepartmentId
                                          )
                                      )
                                      UPDATE {Constants.LOCATION_TABLE_ROUTE} l
                                      SET is_active = FALSE,
                                          deleted_at = NOW()
                                      WHERE l.id IN (SELECT location_id FROM disabled_locations)
                                      RETURNING l.id;
                                  """;

        var conn = _dbContext.Database.GetDbConnection();

        var ids = (await conn.QueryAsync<Guid>(
                dapperSql,
                new { DepartmentId = departmentId.Value }))
            .ToArray();

        return Result.Success<Guid[], Error>(ids);
    }




    public async Task<Result<Guid[], Error>> DeactivateConnectedPositions(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        const string dapperSql = $"""
                                 WITH target_positions AS   
                                    (
                                        SELECT position_id 
                                        FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE}
                                        WHERE department_id = @DepartmentId
                                    ),
                                    disabled_positions AS
                                    (
                                        SELECT position_id
                                        FROM target_positions tp
                                        WHERE NOT EXISTS 
                                        (
                                            SELECT position_id
                                            FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE} dp
                                            JOIN {Constants.DEPARTMENT_TABLE_ROUTE} d ON d.id = dp.department_id
                                            WHERE dp.position_id = tp.position_id
                                            AND d.is_active = TRUE
                                            AND d.id <> @DepartmentId
                                        )
                                    )
                                 UPDATE {Constants.POSITION_TABLE_ROUTE} p
                                 SET 
                                     is_active = FALSE,
                                     deleted_at = NOW()
                                 WHERE p.id IN (SELECT position_id FROM disabled_positions)
                                 RETURNING p.id
                                 """;

        var conn = _dbContext.Database.GetDbConnection();

        var ids = (await conn.QueryAsync<Guid>(
                dapperSql,
                new { DepartmentId = departmentId.Value }))
            .ToArray();

        return Result.Success<Guid[], Error>(ids);
    }


    public async Task<Result<Department, Error>> GetByIdWithLockAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .FromSqlRaw(
                $@"SELECT * 
                   FROM {Constants.DEPARTMENT_TABLE_ROUTE} 
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
            $"""
              SELECT id 
              FROM {Constants.DEPARTMENT_TABLE_ROUTE} 
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
              FROM {{Constants.DEPARTMENT_TABLE_ROUTE}} 
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
              UPDATE {{Constants.DEPARTMENT_TABLE_ROUTE}}
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
            parentId = departmentUpdated.ParentId?.Value,
            newPath = departmentUpdated.Path.Value,
            oldPath = oldPath.Value,
            newDepth = departmentUpdated.Depth,
        });

        return Result.Success<Guid, Error>(departmentUpdated.Id.Value);
    }

}