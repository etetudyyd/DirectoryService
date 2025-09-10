using CSharpFunctionalExtensions;
using TimeZoneConverter;

namespace DevQuestions.Domain.ValueObjects.LocationVO;

public record Timezone
{
    public readonly string Value;

    private Timezone(string value)
    {
        Value = value;
    }

    public static Result<Timezone> Create(string value)
    {
        //Timezone logic

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Timezone>("TimeZone invalid");

        return Result.Success(new Timezone(value));
    }
}