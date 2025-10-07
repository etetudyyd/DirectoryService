using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryBaseTests : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    private IServiceProvider Services { get; }

    protected DirectoryBaseTests(DirectoryTestWebFactory factory)
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

    protected async Task<Guid[]> CreateNLocationsValid(int count)
    {
        var locations = new List<Location>();

        for (int i = 0; i < count; i++)
        {
            var location = Location.Create(
                LocationName.Create($"test-name-{i}").Value,
                Address.Create(
                    $"12345{i}",
                    $"test-region-{i}",
                    $"test-city-{i}",
                    $"test-street-{i}",
                    $"test-house-{i}",
                    $"test-apartment-{i}").Value,
                Timezone.Create("Europe/Berlin").Value,
                []);

            locations.Add(location.Value);
        }

        return await ExecuteInDb<Guid[]>(async dbContext =>
        {
            await dbContext.Locations.AddRangeAsync(locations, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            return locations.Select(l => l.Id.Value).ToArray();
        });
    }

    protected async Task<Guid[]> CreateNLocationsInvalid(int count)
    {
        var locations = new List<Location>();

        for (int i = 0; i < count; i++)
        {
            Result<Location, Error> location = default;

            locations.Add(location.Value);
        }

        return await ExecuteInDb<Guid[]>(async dbContext =>
        {
            await dbContext.Locations.AddRangeAsync(locations, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            return locations.Select(l => l.Id.Value).ToArray();
        });
    }

    protected async Task<Guid> CreateDepartmentParentValid(Guid[] locationsIds)
    {
        var departmentId = new DepartmentId(Guid.NewGuid());

        var departmentLocations = locationsIds
            .Select(locationId => new DepartmentLocation(
                new DepartmentLocationId(Guid.NewGuid()),
                departmentId,
                new LocationId(locationId)))
            .ToList();

        var department = Department.CreateParent(
                DepartmentName.Create($"parent-name").Value,
                Identifier.Create($"parent-identifier").Value,
                departmentLocations,
                departmentId).Value;

        return await ExecuteInDb(async dbContext =>
        {
            await dbContext.Departments.AddAsync(department, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            return department.Id.Value;
        });
    }
}