namespace DirectoryService.Infrastructure;

public class FileServiceBaseClass : IClassFixture<FileServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected IServiceProvider Services { get; init; }

    protected HttpClient AppHttpClient { get; init; }

    protected HttpClient HttpClient { get; init; }

    public FileServiceBaseClass(FileServiceTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        Services = factory.Services;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();
}