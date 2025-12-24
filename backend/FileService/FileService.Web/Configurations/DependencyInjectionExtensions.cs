using DirectoryService;
using Framework.Endpoints;
using Framework.Logging;
using Framework.Swagger;

namespace FileService.Configurations;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSerilogLogging(configuration, "FileService")
            .AddOpenApiSpec("FileService", "v1")
            .AddEndpoints(typeof(DependencyInjectionCoreExtensions).Assembly)
            .AddInfrastructureServices(configuration)
            .AddS3(configuration);

        services
            .AddCore(configuration);

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<FileServiceDbContext>(_ =>
            new FileServiceDbContext(configuration.GetConnectionString("DefaultConnection")!));

        return services;
    }
}