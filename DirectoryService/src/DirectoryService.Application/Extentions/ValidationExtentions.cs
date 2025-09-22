using DevQuestions.Domain.Shared;
using FluentValidation.Results;

namespace DirectoryService.Application.Extentions;

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