using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public sealed record MediaData
{
    public FileName FileName { get; init; }

    public ContentType ContentType { get; init; }

    public long Size { get; init; }

    public int ExpectedChunksCount { get; }

    private MediaData() { }

    private MediaData(
        FileName fileName,
        ContentType contentType,
        long size,
        int expectedChunksCount)
    {
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        ExpectedChunksCount = expectedChunksCount;
    }

    public static Result<MediaData, Error> Create(
        FileName fileName,
        ContentType contentType,
        long size,
        int expectedChunksCount)
    {
        if(size <= 0)
            return GeneralErrors.General.ValueIsInvalid(nameof(size));
        if (expectedChunksCount <= 0)
            return GeneralErrors.General.ValueIsInvalid(nameof(expectedChunksCount));

        return new MediaData(fileName, contentType, size, expectedChunksCount);
    }

}