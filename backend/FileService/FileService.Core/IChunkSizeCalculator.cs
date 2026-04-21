using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public interface IChunkSizeCalculator
{
    Result<(long ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize);
}