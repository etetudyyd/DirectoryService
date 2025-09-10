using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.ValueObjects.LocationVO;

public record LocationName
{
    // TODO Unique Feature
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 100;

    public readonly string Value;
    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length < MIN_LENGTH
            || value.Length > MAX_LENGTH)
        {
            return Result.Failure<LocationName>(
                $"Name is required and must be more than {MIN_LENGTH} characters and less than {MAX_LENGTH} characters");
        }

        return new LocationName(value);
    }
}