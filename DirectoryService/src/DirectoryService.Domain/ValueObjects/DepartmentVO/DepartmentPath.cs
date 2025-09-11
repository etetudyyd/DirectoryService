using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.ValueObjects.DepartmentVO;

public record DepartmentPath
{
    private const string TEXT_PATTERN = "^[a-z-.]+$";

    public string Value { get; }
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public static Result<DepartmentPath> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<DepartmentPath>(
                "Path is required");
        }

        if (!Regex.IsMatch(value, TEXT_PATTERN))
        {
            return Result.Failure<DepartmentPath>(
                "Path is invalid");
        }

        return new DepartmentPath(value);
    }
}