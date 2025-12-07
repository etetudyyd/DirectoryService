using CSharpFunctionalExtensions;

namespace Shared.SharedKernel;

public interface ISoftDeletable
{
    UnitResult<Error> Deactivate();

    UnitResult<Error> Activate();
}