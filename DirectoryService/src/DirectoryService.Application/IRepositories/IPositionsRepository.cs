using DevQuestions.Domain.Entities;

namespace DirectoryService.Application.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);
}