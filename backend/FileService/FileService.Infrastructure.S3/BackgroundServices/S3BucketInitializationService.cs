using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using DirectoryService.FilesStorage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.BackgroundServices;

public class S3BucketInitializationService : BackgroundService
{
    private readonly FileStorageOptions _fileStorageOptions;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3BucketInitializationService> _logger;

    public S3BucketInitializationService(
        ILogger<S3BucketInitializationService> logger,
        IAmazonS3 s3Client,
        IOptions<FileStorageOptions> s3Options)
    {
        _logger = logger;
        _s3Client = s3Client;
        _fileStorageOptions = s3Options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_fileStorageOptions.RequiredBuckets.Count == 0)
            {
                _logger.LogInformation("S3 bucket initialization service required buckets");
                throw new ArgumentException("RequiredBuckets is required");
            }

            _logger.LogInformation(
                "Starting S3 bucket initialization service. Required buckets: {Buckets}",
                string.Join(", ", _fileStorageOptions.RequiredBuckets));

            var tasks = _fileStorageOptions.RequiredBuckets
                .Select(bucket => InitializeBucketsAsync(bucket, stoppingToken))
                .ToArray();

            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("S3 bucket initialization service canceled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "S3 bucket initialization service failed");
            throw;
        }
    }

    private async Task InitializeBucketsAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (bucketExists)
            {
                _logger.LogInformation("Bucket {Bucket} already exists", bucketName);
                return;
            }

            _logger.LogInformation("Creating bucket {Bucket}", bucketName);

            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
            };

            await _s3Client.PutBucketAsync(putBucketRequest, cancellationToken);

            string policy = $$"""
                              {
                                  "Version": "2012-10-17",
                                  "Statement": [
                                      {
                                          "Effect": "Allow",
                                          "Principal": {
                                              "AWS": ["*"]
                                          },
                                      "Action": ["s3:GetObject"],
                                      "Resource": ["arn:aws:s3:::{{bucketName}}/"]
                                      }
                                  ]
                              }
                              """;

            var putBucketPolicyRequest = new PutBucketPolicyRequest
            {
                BucketName = bucketName,
                Policy = policy,
            };

            await _s3Client.PutBucketPolicyAsync(putBucketPolicyRequest, cancellationToken);

            _logger.LogInformation("Bucket {Bucket} created successfully", bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create bucket {Bucket}", bucketName);
            throw;
        }
    }
}