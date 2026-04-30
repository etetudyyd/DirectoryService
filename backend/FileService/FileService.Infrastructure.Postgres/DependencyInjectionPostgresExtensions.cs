using DirectoryService.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryService;

public static class DependencyInjectionPostgresExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddScoped<IMediaAssetsRepository, MediaAssetsRepository>();

       services.AddDbContextPool<FileServiceDbContext>((sp, options) =>
       {
           string? connectionString = configuration.GetConnectionString(Constants.DATABASE);
           IHostingEnvironment env = sp.GetRequiredService<IHostingEnvironment>();
           ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

           options.UseNpgsql(connectionString);

           if (env.IsDevelopment())
           {
               options.EnableSensitiveDataLogging();
               options.EnableDetailedErrors();
           }

           options.UseLoggerFactory(loggerFactory);
       });

       services.AddDbContextPool<IReadDbContext, FileServiceDbContext>((sp, options) =>
       {
           string? connectionString = configuration.GetConnectionString(Constants.DATABASE);
           IHostingEnvironment env = sp.GetRequiredService<IHostingEnvironment>();
           ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

           options.UseNpgsql(connectionString);

           if (env.IsDevelopment())
           {
               options.EnableSensitiveDataLogging();
               options.EnableDetailedErrors();
           }

           options.UseLoggerFactory(loggerFactory);
       });

       return services;
    }
}