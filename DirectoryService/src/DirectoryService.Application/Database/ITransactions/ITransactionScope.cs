using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Database.ITransactions;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit(CancellationToken cancellationToken);

    UnitResult<Error> Rollback(CancellationToken cancellationToken);
}