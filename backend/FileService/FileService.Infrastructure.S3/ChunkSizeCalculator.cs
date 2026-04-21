using CSharpFunctionalExtensions;
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

    public Result<(long ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize)
    {
        int recommendedChunkSize = _s3Options.RecommendedChunkSizeBytes;
        int maxChunks = _s3Options.MaxChunks;

        if(recommendedChunkSize <= 0 || maxChunks <= 0 )
            return GeneralErrors.General.ValueIsInvalid("Invalid chunk size or max chunks");

        if (fileSize <= recommendedChunkSize)
        {
            return (fileSize, 1);
        }

        int calculatedChunks = (int)Math.Ceiling((double)fileSize / recommendedChunkSize);

        int actualChunks = Math.Min(calculatedChunks, maxChunks);

        long chunkSize = fileSize / actualChunks;

        return (chunkSize, actualChunks); 
    }
}