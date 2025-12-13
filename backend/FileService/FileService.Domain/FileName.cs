using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public sealed record FileName
{
    public string Name { get; }

    public string Extension { get; }

    private FileName(string name, string extension)
    {
        Name = name;
        Extension = extension;
    }

    public static Result<FileName, Error> Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return GeneralErrors.General.ValueIsInvalid(nameof(fileName));

        int lastdot = fileName.LastIndexOf('.');
        if (lastdot == -1 || lastdot == fileName.Length - 1)
            return GeneralErrors.General.ValueIsInvalid("File must have extension");

        string extension = fileName[(lastdot + 1)..].ToLowerInvariant();
        return new FileName(fileName, extension);
    }
}