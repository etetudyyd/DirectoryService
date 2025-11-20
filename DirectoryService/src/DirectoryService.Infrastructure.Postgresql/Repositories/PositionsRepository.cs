using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

/// <summary>
/// PositionsRepository - realization of Repository Pattern for Positions database logic by using DirectoryServiceDbContext.
/// It realizes interface IPositionsRepository.
/// </summary>
public class PositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public PositionsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Position position, CancellationToken cancellationToken)
    {
        await _dbContext.Positions.AddAsync(position, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return position.Id.Value;
    }

    public async Task<UnitResult<Error>> BulkDeleteInactivePositionsAsync(
        List<DepartmentId> departmentIds,
        CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        var ids = departmentIds.Select(d => d.Value).ToArray();

        string sql = $@"
        WITH deleted_dp AS (
            DELETE FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE}
            WHERE department_id = ANY(@Ids)
            RETURNING 1
        ),
        deleted_positions AS (
            DELETE FROM {Constants.POSITION_TABLE_ROUTE} p
            WHERE p.is_active = FALSE
              AND NOT EXISTS (
                  SELECT 1
                  FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE} dp
                  WHERE dp.position_id = p.id
              )
            RETURNING 1
        )
        SELECT 1;
    ";

        await connection.ExecuteAsync(sql, new { Ids = ids });

        return UnitResult.Success<Error>();
    }


    public async Task<UnitResult<Error>> DeleteInactiveAsync(CancellationToken cancellationToken)
    {
        string sql = $@"
        DELETE FROM {Constants.POSITION_TABLE_ROUTE}
        WHERE is_active = false
          AND NOT EXISTS (
                SELECT 1
                FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE} dp
                WHERE dp.position_id = {Constants.POSITION_TABLE_ROUTE}.id
          );
    ";

        await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);

        return UnitResult.Success<Error>();
    }

}