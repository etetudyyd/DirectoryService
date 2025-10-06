using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryBaseTests(DirectoryTestWebFactory factory)
    : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    protected IServiceProvider Services { get; set; } = factory.Services;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(dbContext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await action(dbContext);
    }

    protected async Task<TResult> ExecuteCommand<TService, TResult>(Func<TService, Task<TResult>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(service);
    }
}