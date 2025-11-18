using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Database.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteInactiveAsync(CancellationToken cancellationToken);

}