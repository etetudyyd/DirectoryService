using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public sealed record ContentType
{
    public string Value { get; }

    public MediaType Category { get; }

    private ContentType(string value, MediaType mediaType)
    {
        Value = value;
        Category = mediaType;
    }

    public static Result<ContentType, Error> Create(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return GeneralErrors.General.ValueIsInvalid(nameof(contentType));

        MediaType category = contentType switch
        {
            _ when contentType.Contains("video", StringComparison.InvariantCultureIgnoreCase) => MediaType.VIDEO,
            _ when contentType.Contains("image", StringComparison.InvariantCultureIgnoreCase) => MediaType.IMAGE,
            _ when contentType.Contains("audio", StringComparison.InvariantCultureIgnoreCase) => MediaType.AUDIO,
            _ when contentType.Contains("document", StringComparison.InvariantCultureIgnoreCase) => MediaType.DOCUMENT,
            _ => MediaType.UNKNOWN,
        };

        return new ContentType(contentType, category);
    }
}