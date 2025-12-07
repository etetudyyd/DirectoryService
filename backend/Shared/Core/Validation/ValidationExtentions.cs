using Shared.SharedKernel;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Core.Validation;

public static class ValidationExtentions
{
    public static Errors ToErrors(this ValidationResult validationResult)
    {
        return new Errors(
            validationResult.Errors.Select(e =>
                    Error.Validation(
                        e.ErrorCode,
                        e.ErrorMessage,
                        e.PropertyName))
                .ToArray());
    }

}