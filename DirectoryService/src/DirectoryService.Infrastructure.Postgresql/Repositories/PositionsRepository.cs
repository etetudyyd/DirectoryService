using CSharpFunctionalExtensions;
using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
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