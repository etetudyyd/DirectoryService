﻿using DevQuestions.Domain.Entities;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Infrastructure.Postgresql.Database;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

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
}