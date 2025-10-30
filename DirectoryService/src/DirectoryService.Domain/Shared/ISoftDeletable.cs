using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DevQuestions.Domain.Entities;

public interface ISoftDeletable
{
    UnitResult<Error> Delete();

    UnitResult<Error> Restore();
}