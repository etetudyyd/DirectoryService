using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Database.ITransactions;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}