using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService;

public static class DependencyInjectionS3Extensions
{
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration.GetSection(nameof(S3Options)));

        S3Options s3Options = configuration.GetSection(nameof(S3Options)).Get<S3Options>()
            ?? throw new ApplicationException($"{nameof(S3Options)} is required.");

        var options = new AWSOptions
        {
            DefaultClientConfig =
                {
                    ServiceURL = s3Options.Endpoint,
                    UseHttp = !s3Options.WithSsl,
                },
            Credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey),
        };

        services.AddAWSService<IAmazonS3>();

        return services;
    }
}