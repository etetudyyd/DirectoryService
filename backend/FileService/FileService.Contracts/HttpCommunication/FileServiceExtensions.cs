using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DirectoryService.HttpCommunication;

public static class FileServiceExtensions
{
    public static IServiceCollection AddFileServiceHttpCommunication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileServiceOptions>(configuration.GetSection(nameof(FileServiceOptions)));
        services.AddHttpClient<IFileCommunicationService, FileHttpClient>((sp, config) =>
        {
            FileServiceOptions fileServiceOptions = sp.GetRequiredService<IOptions<FileServiceOptions>>().Value;

            config.BaseAddress = new Uri(fileServiceOptions.Url);
            config.Timeout = TimeSpan.FromSeconds(fileServiceOptions.TimeoutSeconds);
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}