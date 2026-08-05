using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using DirectoryService.BackgroundServices;
using DirectoryService.FilesStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DirectoryService;

public static class DependencyInjectionS3Extensions
{
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection(nameof(FileStorageOptions)));

        services.AddScoped<IS3Provider, S3Provider>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            FileStorageOptions fileStorageOptions = sp.GetRequiredService<IOptions<FileStorageOptions>>().Value;

            var config = new AmazonS3Config
            {
              ServiceURL = fileStorageOptions.Endpoint, UseHttp = !fileStorageOptions.WithSsl, ForcePathStyle = true,
            };

            return new AmazonS3Client(fileStorageOptions.AccessKey, fileStorageOptions.SecretKey, config);
        });

        services.AddHostedService<S3BucketInitializationService>();

        services.AddTransient<IChunkSizeCalculator, ChunkSizeCalculator>();

        return services;
    }
}