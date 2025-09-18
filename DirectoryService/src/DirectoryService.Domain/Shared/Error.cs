using System.Text.Json.Serialization;

namespace DevQuestions.Domain.Shared;

public record Error
{
    private const string SEPARATOR = "||";

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public string Code { get; }

    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    public static Error Validation(string? code, string message, string? invalidField = null) =>
        new(code ?? "validation.error", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message) => new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string message) => new(code ?? "failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message) => new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        string[] parts = serialized.Split(SEPARATOR);

        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        return new Error(parts[0], parts[1], type);
    }

    public Errors ToErrors() => new([this]);
}