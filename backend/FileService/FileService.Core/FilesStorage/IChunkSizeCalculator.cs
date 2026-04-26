using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.FilesStorage;

public interface IChunkSizeCalculator
{
    Result<(long ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize);
}