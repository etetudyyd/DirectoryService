using DevQuestions.Domain.Entities;

namespace DirectoryService.Application.Database.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);
}