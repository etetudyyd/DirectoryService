using CSharpFunctionalExtensions;
using DirectoryService.Responses;
using Shared.SharedKernel;

namespace DirectoryService;

public interface IS3Provider
{
    Task<UnitResult<Error>> UploadFileAsync(StorageKey key, Stream stream, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> DownloadFileAsync(StorageKey key, CancellationToken cancellationToken);

    Task<Result<string, Error>> DeleteFileAsync(StorageKey key, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateUploadUrlAsync(StorageKey key, MediaData mediaData);

    Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey key);

    Task<Result<IReadOnlyList<string>, Errors>> GenerateDownloadUrlsAsync(IEnumerable<StorageKey> keys);

    Task<Result<string, Error>> StartMultipartUpload(StorageKey key, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateChunkUploadUrl(StorageKey key, string uploadId, int partNumber);

    Task<Result<IReadOnlyList<string>, Error>> GenerateAllChunkUploadUrls(StorageKey key, string uploadId, int totalChunks, CancellationToken cancellationToken);

    Task<UnitResult<Error>> CompleteMultipartUploadAsync(StorageKey key, string uploadId, List<PartETagDto> eTags, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AbortMultipartUploadAsync(StorageKey key, string uploadId, CancellationToken cancellationToken);

    Task<Result<ListMultipartUploadsResponse, Error>> ListMultipartUploadAsync(string bucketName, CancellationToken cancellationToken);
}