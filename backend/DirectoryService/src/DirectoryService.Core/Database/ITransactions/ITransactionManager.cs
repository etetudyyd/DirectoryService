using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.Database.ITransactions;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}