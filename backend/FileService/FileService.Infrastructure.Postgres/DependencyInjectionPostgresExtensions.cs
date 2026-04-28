using DirectoryService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryService;

public static class DependencyInjectionPostgresExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<FileServiceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole())));

        services.AddScoped<IReadDbContext>(sp =>
            sp.GetRequiredService<FileServiceDbContext>());

        services.AddScoped<IMediaAssetsRepository, MediaAssetsRepository>();

        return services;
    }
}