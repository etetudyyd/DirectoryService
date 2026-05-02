using Amazon.S3;
using DirectoryService.FilesStorage;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public class FileServiceBaseClass : IClassFixture<FileServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    private readonly IServiceProvider _serviceProvider;

    protected HttpClient AppHttpClient { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected FileServiceBaseClass(FileServiceTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        _serviceProvider = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _resetDatabase();
        AppHttpClient.Dispose();
        HttpClient.Dispose();
    }

    private async Task<TResult> Execute<TResult, TService>(Func<TService, Task<TResult>> action)
        where TService : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TService handler = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(handler);
    }

    private async Task Execute<TService>(Func<TService, Task> action)
        where TService : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TService handler = scope.ServiceProvider.GetRequiredService<TService>();
        await action(handler);
    }

    protected async Task<T> ExecuteHandler<TCommand, T>(Func<TCommand, Task<T>> action)
        where TCommand : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TCommand sut = scope.ServiceProvider.GetRequiredService<TCommand>();
        return await action(sut);
    }

    protected async Task<T> ExecuteInDb<T>(Func<FileServiceDbContext, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInDb(Func<FileServiceDbContext, Task> action) => await Execute(action);

    protected async Task<T> ExecuteInS3Client<T>(Func<IAmazonS3, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInS3Client(Func<IAmazonS3, Task> action) => await Execute(action);

    protected async Task<T> ExecuteInS3Provider<T>(Func<IS3Provider, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInS3Provider(Func<IS3Provider, Task> action) => await Execute(action);

}