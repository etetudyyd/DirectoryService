using Amazon.S3;

namespace DirectoryService;

public class S3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _s3Options;

    public S3Provider(IAmazonS3 s3Client, S3Options s3Options)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
    }

    public async Task UploadFile(string bucketName, string key, string fileName)
    {
        
    }
}