using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public interface IS3Provider
{
    Task<UnitResult<Error>> UploadFileAsync(StorageKey key, Stream stream, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> DownloadFileAsync(StorageKey key, string tempPath);

    Task<Result<string, Error>> DeleteFileAsync(StorageKey key, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateUploadUrlAsync(StorageKey key, MediaData mediaData, CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey key);

    Task<Result<IReadOnlyList<string>, Error>> GenerateDownloadUrlsAsync(IEnumerable<StorageKey> keys);
}