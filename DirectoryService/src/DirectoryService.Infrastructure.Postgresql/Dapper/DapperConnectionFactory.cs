using System.Data;
using Dapper;
using DirectoryService.Application.Database.IQueries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgresql.Dapper;

public class DapperConnectionFactory : IDapperConnectionFactory, IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<DapperConnectionFactory> _logger;

    public DapperConnectionFactory(
        IConfiguration configuration,
        ILogger<DapperConnectionFactory> logger)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.UseLoggerFactory(CreateLoggerFactory());
        _dataSource = dataSourceBuilder.Build();
        _logger = logger;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(@"SET search_path TO ""DirectoryService"", public;");

            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to database with Dapper");
            throw;
        }
    }


    private static ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(builder => builder.AddConsole());

    public void Dispose() => _dataSource?.Dispose();

    public async ValueTask DisposeAsync() => await _dataSource.DisposeAsync();
}