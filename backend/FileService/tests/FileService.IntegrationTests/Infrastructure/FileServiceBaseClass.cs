using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public class FileServiceBaseClass : IClassFixture<FileServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected IServiceProvider Services { get; init; }

    protected HttpClient AppHttpClient { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected FileServiceBaseClass(FileServiceTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        Services = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task<T> ExecuteInDb<T>(Func<FileServiceDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();
        return await action(dbContext);
    }

    protected async Task ExecuteInDb(Func<FileServiceDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();
        await action(dbContext);
    }

    protected async Task<T> ExecuteHandler<TCommand, T>(Func<TCommand, Task<T>> action)
        where TCommand : notnull
    {
        using var scope = Services.CreateScope();

        var sut = scope.ServiceProvider.GetRequiredService<TCommand>();

        return await action(sut);
    }
}