using System.Collections;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SharedKernel;

namespace DirectoryService;

public class S3Provider : IS3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _s3Options;
    private readonly ILogger<S3Provider> _logger;
    private readonly SemaphoreSlim _requestsSemaphore;

    public S3Provider(
        IAmazonS3 s3Client,
        IOptions<S3Options> s3Options,
        ILogger<S3Provider> logger)
    {
        _s3Client = s3Client;
        _s3Options = s3Options.Value;
        _logger = logger;
        _requestsSemaphore = new SemaphoreSlim(_s3Options.MaxConcurrentRequests);
    }

    public async Task<UnitResult<Error>> UploadFileAsync(
        StorageKey storageKey,
        Stream stream,
        MediaData mediaData,
        CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = storageKey.Bucket,
            Key = storageKey.Value,
            ContentType = mediaData.ContentType.Value,
            InputStream = stream,
        };

        try
        {
            await _s3Client.PutObjectAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return S3ErrorMapper.ToError(ex);
        }

        return Result.Success<Error>();
    }

    public async Task<Result<string, Error>> DownloadFileAsync(
        StorageKey key,
        CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = key.Bucket,
            Key = key.Key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.DownloadUrlExpirationHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        try
        {
            string? response = await _s3Client.GetPreSignedURLAsync(request);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateUploadUrlAsync(
        StorageKey storageKey,
        MediaData mediaData,
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

        try
        {
            string? response = await _s3Client.GetPreSignedURLAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generate upload url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> DeleteFileAsync(
        StorageKey key,
        CancellationToken cancellationToken)
    {
        var request = new DeleteObjectRequest { BucketName = key.Bucket };

        try
        {
            DeleteObjectResponse response = await _s3Client.DeleteObjectAsync(request, cancellationToken);
            return response.DeleteMarker;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delete file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = key.Bucket,
            Key = key.Key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        try
        {
             string? response = await _s3Client.GetPreSignedURLAsync(request);
             return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generate download url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<string>, Errors>> GenerateDownloadUrlsAsync(IEnumerable<StorageKey> keys)
    {
            IEnumerable<Task<Result<string, Error>>> tasks = keys
                .Select(async key =>
                {
                    await _requestsSemaphore.WaitAsync();

                    try
                    {
                      return await GenerateDownloadUrlAsync(key);
                    }
                    finally
                    {
                        _requestsSemaphore.Release();
                    }
                });

            Result<string, Error>[] downloadUrlsResult = await Task.WhenAll(tasks);

            Error[] errors = downloadUrlsResult
                .Where(res => res.IsFailure)
                .Select(res => res.Error)
                .ToArray();

            if (errors.Any())
            {
                _logger.LogError("Error generate download urls");
                return new Errors(errors);
            }

            return downloadUrlsResult.Select(res => res.Value).ToList();
    }
}