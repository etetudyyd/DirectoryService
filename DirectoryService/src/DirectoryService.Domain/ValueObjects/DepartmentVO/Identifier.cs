using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.ValueObjects.DepartmentVO;

public record Identifier
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 150;
    private const string TEXT_PATTERN = "^[a-z-.]+$";

    public string Value { get; }

    private Identifier(string value)
    {
        Value = value;
    }

    public static Result<Identifier> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_LENGTH
            || value.Length > MAX_LENGTH)
        {
            return Result.Failure<Identifier>(
                $"Identifier is required and must be more than {MIN_LENGTH} characters and less than {MAX_LENGTH} characters");
        }

        if (!Regex.IsMatch(value, TEXT_PATTERN))
        {
            return Result.Failure<Identifier>(
                $"Identifier is required and must match pattern {TEXT_PATTERN}");
        }

        return new Identifier(value);
    }
}