using CSharpFunctionalExtensions;
using DirectoryService.Models;
using DirectoryService.Responses;
using DirectoryService.VOs;
using Shared.SharedKernel;

namespace DirectoryService.FilesStorage;

public interface IS3Provider
{
    Task<UnitResult<Error>> UploadFileAsync(StorageKey key, Stream stream, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> DownloadFileAsync(StorageKey key, CancellationToken cancellationToken);

    Task<Result<string, Error>> DeleteFileAsync(StorageKey key, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateUploadUrlAsync(StorageKey key, MediaData mediaData);

    Task<Result<MediaUrl, Error>> GenerateDownloadUrlAsync(StorageKey key);

    Task<Result<IReadOnlyList<MediaUrl>, Errors>> GenerateDownloadUrlsAsync(IEnumerable<StorageKey> keys);

    Task<Result<string, Error>> StartMultipartUploadAsync(StorageKey key, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateChunkUploadUrl(StorageKey key, string uploadId, int partNumber);

    Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunkUploadUrls(StorageKey key, string uploadId, int totalChunks, CancellationToken cancellationToken);

    Task<UnitResult<Error>> CompleteMultipartUploadAsync(StorageKey key, string uploadId,
        IReadOnlyList<PartETagDto> eTags, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AbortMultipartUploadAsync(StorageKey key, string uploadId, CancellationToken cancellationToken);
}