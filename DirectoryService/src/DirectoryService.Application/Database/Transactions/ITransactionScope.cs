using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Database.Transactions;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}