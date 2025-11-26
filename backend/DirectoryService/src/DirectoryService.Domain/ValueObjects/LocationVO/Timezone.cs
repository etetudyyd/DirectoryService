using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using TimeZoneConverter;

namespace DevQuestions.Domain.ValueObjects.LocationVO;

public record Timezone
{
    public string Value { get; }

    private Timezone(string value)
    {
        Value = value;
    }

    public static Result<Timezone, Error> Create(string value)
    {
        //Timezone logic

        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null,"TimeZone invalid");

        return new Timezone(value);
    }
}