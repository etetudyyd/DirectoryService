using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryBaseTests : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected IServiceProvider Services { get; }

    public DirectoryBaseTests(DirectoryTestWebFactory factory)
    {
        Services = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(dbContext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await action(dbContext);
    }

    protected async Task<T> ExecuteHandler<TCommand, T>(Func<TCommand, Task<T>> action)
        where TCommand : notnull
    {
        using var scope = Services.CreateScope();

        var sut = scope.ServiceProvider.GetRequiredService<TCommand>();

        return await action(sut);
    }

    protected async Task<LocationId> CreateOneLocation()
    {
        var timezone = Timezone.Create("Europe/Berlin");
        var name = LocationName.Create("test-name");
        var address = Address.Create(
            "123456",
            "test-region",
            "test-city",
            "test-street",
            "test-house",
            "test-apartment");
        var location = Location.Create(
            name.Value,
            address.Value,
            timezone.Value,
            []);

        return await ExecuteInDb(async dbContext =>
        {
            await dbContext.Locations.AddAsync(location.Value, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            return location.Value.Id;
        });
    }
}