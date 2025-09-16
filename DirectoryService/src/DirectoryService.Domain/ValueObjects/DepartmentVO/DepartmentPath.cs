using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DevQuestions.Domain.ValueObjects.DepartmentVO;

public record DepartmentPath
{
    private const string TEXT_PATTERN = "^[a-z-.]+$";

    public string Value { get; }
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public static Result<DepartmentPath, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation(
                null,
                "Path is required");
        }

        if (!Regex.IsMatch(value, TEXT_PATTERN))
        {
            return Error.Validation(
                null,
                "Path is invalid");
        }

        return new DepartmentPath(value);
    }
}