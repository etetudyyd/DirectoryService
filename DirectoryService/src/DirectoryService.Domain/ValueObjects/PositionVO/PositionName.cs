﻿using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DevQuestions.Domain.ValueObjects.PositionVO;

public record PositionName
{
    // TODO Unique Feature
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 100;

    public string Value { get; }
    private PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length < MIN_LENGTH
            || value.Length > MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Name is required and must be more than {MIN_LENGTH} characters and less than {MAX_LENGTH} characters");
        }

        return new PositionName(value);
    }
}