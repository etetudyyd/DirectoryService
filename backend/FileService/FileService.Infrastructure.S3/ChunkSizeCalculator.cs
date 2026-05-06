using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using Microsoft.Extensions.Options;
using Shared.SharedKernel;

namespace DirectoryService;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly S3Options _s3Options;

    public ChunkSizeCalculator(IOptions<S3Options> s3Options)
    {
        _s3Options = s3Options.Value;
    }

    public Result<(int ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize)
    {
        if(_s3Options.RecommendedChunkSizeBytes <= 0 || _s3Options.MaxChunks <= 0 )
            return GeneralErrors.General.ValueIsInvalid("Invalid chunk size or max chunks");

        if (fileSize <= _s3Options.RecommendedChunkSizeBytes)
        {
            return ((int)fileSize, 1);
        }

        int calculatedChunks = (int)Math.Ceiling((double)fileSize / _s3Options.RecommendedChunkSizeBytes);

        int actualChunks = Math.Min(calculatedChunks, _s3Options.MaxChunks);

        long chunkSize = (fileSize + actualChunks - 1) / actualChunks;

        return ((int)chunkSize, actualChunks);
    }
}