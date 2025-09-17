using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DevQuestions.Domain.ValueObjects.DepartmentVO;

public sealed record Path
{
    private const char SEPARATOR = '/';

    public string Value { get; }

    private Path(string value)
    {
        Value = value;
    }

    public static Path CreateParent(Identifier identifier)
    {
        return new Path(identifier.Value);
    }

    public Path CreateChild(Identifier childIdentifier)
    {
        return new Path(Value + SEPARATOR + childIdentifier.Value);
    }
}