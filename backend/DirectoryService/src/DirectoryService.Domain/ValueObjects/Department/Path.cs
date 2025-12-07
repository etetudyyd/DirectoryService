using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.ValueObjects.Department;

public sealed record Path
{
    // private const char SEPARATOR = '/';
    private const char SEPARATOR = '.';

    public string Value { get; }

    private Path(string value)
    {
        Value = value;
    }

    public static Result<Path, Error> CalculatePath(Path? parentPath, Identifier identifier)
    {
        if (parentPath == null)
        {
            var pathCreateResult = Create(identifier.Value);
            if (!pathCreateResult.IsSuccess)
                return pathCreateResult.Error;
            return pathCreateResult.Value;
        }

        string newPathString = parentPath.Value + "." + identifier.Value;
        var newPathCreateResult = Create(newPathString);
        if (!newPathCreateResult.IsSuccess)
            return newPathCreateResult.Error;
        return newPathCreateResult.Value;
    }

    public static Path CreateParent(Identifier identifier)
    {
        return new Path(identifier.Value);
    }
    public static Path CreateForDb(string value)
    {
        return new Path(value);
    }

    public Path CreateChild(Identifier childIdentifier)
    {
        return new Path(Value + SEPARATOR + childIdentifier.Value);
    }

    public static Result<Path, Error> Create(string value)
    {
        if(value is null)
            return Error.Failure("value.is.null", "Value cannot be null");

        return new Path(value.ToLower());
    }

    public static Path CreateDeleted(string oldIdentifier, Path path)
    {
        var newPath = path.Value.Replace(
            oldIdentifier,
            $"{Constants.SOFT_DELETED_LABEL}{oldIdentifier}");
        return new Path(newPath);
    }

    public static Path CreateRestored(string oldIdentifier, Path path)
    {
        var newPath = path.Value.Replace(
            $"{Constants.SOFT_DELETED_LABEL}{oldIdentifier}",
            oldIdentifier);
        return new Path(newPath);
    }
}