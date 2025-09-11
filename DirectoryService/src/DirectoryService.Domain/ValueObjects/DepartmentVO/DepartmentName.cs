using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.ValueObjects.DepartmentVO;

public record DepartmentName(string Value)
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 150;

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length < MIN_LENGTH
            || value.Length > MAX_LENGTH)
        {
            return Result.Failure<DepartmentName>(
                $"Name is required and must be more than {MIN_LENGTH} characters and less than {MAX_LENGTH} characters");
        }

        return new DepartmentName(value);
    }
}