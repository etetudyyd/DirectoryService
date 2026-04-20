/*using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public static class ChunkSizeCalculator
{
    public static Result<(long chunkSize, int totalChunks), Error> Calculate(
        long fileSize,
        int recommendedChunkSize,
        int maxChunks)
    {
        if (fileSize <= recommendedChunkSize)
        {
            return new (fileSize, 1);
        }

        var chunkSize = ;
        var totalChunks = (fileSize + chunkSize - 1) / chunkSize;
        
        
    }
}*/