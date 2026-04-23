using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using DirectoryService.FilesStorage;
using DirectoryService.VOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SharedKernel;
using ListMultipartUploadsResponse = DirectoryService.Responses.ListMultipartUploadsResponse;

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
        StorageKey key, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(key.Bucket, key.Key, cancellationToken);

            return response.BucketName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> DeleteFileAsync(
        StorageKey key,
        CancellationToken cancellationToken)
    {
        var request = new DeleteObjectRequest { BucketName = key.Bucket, Key = key.Value };

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

    public async Task<Result<string, Error>> GenerateUploadUrlAsync(
        StorageKey storageKey,
        MediaData mediaData)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = storageKey.Bucket,
            Key = storageKey.Key,
            Verb = HttpVerb.PUT,
            ContentType = mediaData.ContentType.Value,
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

    public async Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey key)
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

    public async Task<Result<string, Error>> StartMultipartUploadAsync(StorageKey key, MediaData mediaData,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new InitiateMultipartUploadRequest
            {
                BucketName = key.Bucket,
                Key = key.Key,
                ContentType = mediaData.ContentType.Value,
            };

            var result = await _s3Client.InitiateMultipartUploadAsync(request, cancellationToken);

            return result.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting multipart upload");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateChunkUploadUrl(StorageKey key, string uploadId, int partNumber)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = key.Bucket,
                Key = key.Key,
                Verb = HttpVerb.PUT,
                UploadId = uploadId,
                PartNumber = partNumber,
                Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
                Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
            };

            await _s3Client.GetPreSignedURLAsync(request);

            return request.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generate chunk upload url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunkUploadUrls(
        StorageKey key,
        string uploadId,
        int totalChunks,
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Task<ChunkUploadUrl>> tasks = Enumerable.Range(1, totalChunks)
                .Select(async partNumber =>
                {
                    await _requestsSemaphore.WaitAsync(cancellationToken);

                    try
                    {
                        var request = new GetPreSignedUrlRequest
                        {
                            BucketName = key.Bucket,
                            Key = key.Key,
                            Verb = HttpVerb.PUT,
                            UploadId = uploadId,
                            PartNumber = partNumber,
                            Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
                            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                        };

                        string? url = await _s3Client.GetPreSignedURLAsync(request);

                        return new ChunkUploadUrl(partNumber, url);
                    }
                    finally
                    {
                        _requestsSemaphore.Release();
                    }
                });

            ChunkUploadUrl[] results = await Task.WhenAll(tasks);

            return results;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generate chunk upload url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<UnitResult<Error>> CompleteMultipartUploadAsync(
        StorageKey key,
        string uploadId,
        List<PartETagDto> eTags,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new CompleteMultipartUploadRequest
            {
                BucketName = key.Bucket,
                Key = key.Key,
                UploadId = uploadId,
                PartETags = eTags.Select(p => new PartETag
                {
                    ETag = p.ETag,
                    PartNumber = p.PartNumber,
                }).ToList(),
            };

            await _s3Client.CompleteMultipartUploadAsync(request, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error complete upload url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public Task<UnitResult<Error>> AbortMultipartUploadAsync(StorageKey key, string uploadId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<Result<ListMultipartUploadsResponse, Error>> ListMultipartUploadAsync(string bucketName, CancellationToken cancellationToken) => throw new NotImplementedException();
}