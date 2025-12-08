using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.ValueObjects.Department;

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

    public static Result<Identifier, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_LENGTH
            || value.Length > MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Identifier is required and must be more than {MIN_LENGTH} characters and less than {MAX_LENGTH} characters");
        }

        if (!Regex.IsMatch(value, TEXT_PATTERN))
        {
            return Error.Validation(
                null,
                $"Identifier is required and must match pattern {TEXT_PATTERN}");
        }

        return new Identifier(value);
    }

    public static Result<Identifier, Error> CreateDeleted(Identifier identifier)
    {
        return new Identifier(Constants.SOFT_DELETED_LABEL + identifier.Value);
    }

    public static Result<Identifier, Error> CreateRestored(Identifier identifier)
    {
        return new Identifier(
            identifier.Value.Replace(
            $"{Constants.SOFT_DELETED_LABEL}{identifier.Value}",
            identifier.Value));
    }
}