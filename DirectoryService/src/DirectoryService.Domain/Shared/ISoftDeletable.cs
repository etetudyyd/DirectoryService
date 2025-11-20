using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.Shared;

public interface ISoftDeletable
{
    UnitResult<Error> Deactivate();

    UnitResult<Error> Activate();
}