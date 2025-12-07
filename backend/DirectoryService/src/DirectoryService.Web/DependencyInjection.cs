using DirectoryService.Controllers;

namespace DirectoryService;

/// <summary>
/// DependencyInjection - service for adding scopes in DI web services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers()
            .AddApplicationPart(typeof(LocationsController).Assembly);
        serviceCollection.AddOpenApi();

        return serviceCollection;
    }

    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis")
                                ?? throw new ArgumentNullException(nameof(connection));

            options.Configuration = connection;
        });

        services.AddHybridCache();

        return services;
    }
}