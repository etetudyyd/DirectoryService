using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DevQuestions.Domain.ValueObjects.PositionVO;

public record Description
{
    private const int MAX_LENGTH = 1000;

    public string Value { get; }
    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Name is required and must be less than {MAX_LENGTH} characters");
        }

        return new Description(value);
    }
}
