using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public class S3Provider : IS3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _s3Options;

    public S3Provider(IAmazonS3 s3Client, S3Options s3Options)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
    }

    public async Task<UnitResult<Error>> UploadFileAsync(StorageKey storageKey, Stream stream, MediaData mediaData,
        CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = storageKey.Bucket,
            Key = storageKey.Value,
            ContentType = mediaData.ContentType.Value,
            InputStream = stream,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return Result.Success<Error>();
    }

    public async Task<Result<string, Error>> DownloadFileAsync(StorageKey storageKey, string tempPath)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = storageKey.Bucket,
            Key = storageKey.Key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.DownloadUrlExpirationHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        string? response = await _s3Client.GetPreSignedURLAsync(request);

        return response;
    }

    public async Task<Result<string, Error>> GenerateUploadUrlAsync(StorageKey storageKey, MediaData mediaData,
        CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = storageKey.Bucket,
            Key = storageKey.Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        string? response = await _s3Client.GetPreSignedURLAsync(request);

        return response;
    }

    public Task<Result<string, Error>> DeleteFileAsync(StorageKey key, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey key) => throw new NotImplementedException();

    public Task<Result<IReadOnlyList<string>, Error>> GenerateDownloadUrlsAsync(IEnumerable<StorageKey> keys) => throw new NotImplementedException();
}