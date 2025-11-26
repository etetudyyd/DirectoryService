using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Infrastructure.Postgresql.BackgroundServices;
using DirectoryService.Infrastructure.Postgresql.Dapper;
using DirectoryService.Infrastructure.Postgresql.Database;
using DirectoryService.Infrastructure.Postgresql.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgresql;

/// <summary>
/// DependencyInjection - service for adding scopes in DI infrastructure services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DefaultConnection")!));

        services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<DirectoryServiceDbContext>());

        services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
        services.AddScoped<IDapperConnectionFactory, DapperConnectionFactory>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        services.AddHostedService<InactiveDepartmentsCleanerBackgroundService>();

        return services;
    }
}