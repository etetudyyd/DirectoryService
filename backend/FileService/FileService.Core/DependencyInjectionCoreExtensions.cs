using Core.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService;

public static class DependencyInjectionCoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjectionCoreExtensions).Assembly,
            includeInternalTypes: true);

        var assembly = typeof(DependencyInjectionCoreExtensions).Assembly;

        services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(
                        typeof(ICommandHandler<,>),
                        typeof(ICommandHandler<>),
                        typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

        services.AddStackExchangeRedisCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis")
                                ?? throw new ArgumentNullException(nameof(connection));

            options.Configuration = connection;
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30),
            };
        });

        return services;
    }
}