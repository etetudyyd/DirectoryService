using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.ValueObjects.PositionVO;

public record PositionDescription
{
    private const int MAX_LENGTH = 1000;

    public readonly string Value;
    private PositionDescription(string value)
    {
        Value = value;
    }

    public static Result<PositionDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_LENGTH)
        {
            return Result.Failure<PositionDescription>(
                $"Name is required and must be less than {MAX_LENGTH} characters");
        }

        return new PositionDescription(value);
    }
}
