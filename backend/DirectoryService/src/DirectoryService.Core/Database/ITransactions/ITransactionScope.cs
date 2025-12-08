using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.Database.ITransactions;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit(CancellationToken cancellationToken);

    UnitResult<Error> Rollback(CancellationToken cancellationToken);
}