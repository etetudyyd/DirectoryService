using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using Microsoft.Extensions.Options;
using Shared.SharedKernel;

namespace DirectoryService;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly FileStorageOptions _fileStorageOptions;

    public ChunkSizeCalculator(IOptions<FileStorageOptions> s3Options)
    {
        _fileStorageOptions = s3Options.Value;
    }

    public Result<(int ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize)
    {
        if(_fileStorageOptions.RecommendedChunkSizeBytes <= 0 || _fileStorageOptions.MaxChunks <= 0 )
            return GeneralErrors.General.ValueIsInvalid("Invalid chunk size or max chunks");

        if (fileSize <= _fileStorageOptions.RecommendedChunkSizeBytes)
        {
            return ((int)fileSize, 1);
        }

        int calculatedChunks = (int)Math.Ceiling((double)fileSize / _fileStorageOptions.RecommendedChunkSizeBytes);

        int actualChunks = Math.Min(calculatedChunks, _fileStorageOptions.MaxChunks);

        long chunkSize = (fileSize + actualChunks - 1) / actualChunks;

        return ((int)chunkSize, actualChunks);
    }
}