using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.FilesStorage;

public interface IChunkSizeCalculator
{
    Result<(int ChunkSize, int TotalChunks), Error> Calculate(
        long fileSize);
}