using System.Data.Common;
using Amazon.S3;
using FileService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace DirectoryService.Infrastructure;

public class FileServiceTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("file_service_db_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _minioContainer.StartAsync();

        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();

        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();

        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.Tests.json"), optional:true);
        });

        builder.ConfigureTestServices(services =>
        {
            services.ConfigureDbContext(_dbContainer.GetConnectionString());

            services.ConfigureS3($"http://{_minioContainer.Hostname}:{_minioContainer.GetMappedPublicPort(9000)}");
        });
    }

    private async Task InitializeRespawner()
    {
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["FileService"],
            });
    }
}

public static class ConfiguratorDbContext
{
    public static void ConfigureDbContext(this IServiceCollection services, string connectionString)
    {
        services.RemoveAll<FileServiceDbContext>();
        services.RemoveAll<IReadDbContext>();

        services.AddDbContextPool<FileServiceDbContext>((_, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddDbContextPool<IReadDbContext, FileServiceDbContext>((_, options) =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}

public static class ConfiguratorS3
{
    public static void ConfigureS3(this IServiceCollection services, string serviceUrl)
    {
        services.RemoveAll<IAmazonS3>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            S3Options s3Options = sp.GetRequiredService<IOptions<S3Options>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                UseHttp = true,
                ForcePathStyle = true,
            };

            return new AmazonS3Client(s3Options.AccessKey, s3Options.SecretKey, config);
        });
    }
}