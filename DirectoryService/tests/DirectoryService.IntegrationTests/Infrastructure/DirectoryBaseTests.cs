using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
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
                new LocationId(Guid.NewGuid()),
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

    protected async Task<Guid[]> CreateNLocationsToUpdateValid(int count)
    {
        var locations = new List<Location>();

        for (int i = 0; i < count; i++)
        {
            var location = Location.Create(
                new LocationId(Guid.NewGuid()),
                LocationName.Create($"test-update-name-{i}").Value,
                Address.Create(
                    $"12345{i}",
                    $"test-update-region-{i}",
                    $"test-update-city-{i}",
                    $"test-update-street-{i}",
                    $"test-update-house-{i}",
                    $"test-update-apartment-{i}").Value,
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

    protected async Task<Result<Guid[], Error>> CreateNLocationsInvalid(int count)
    {
        var locations = new List<Location>();

        for (int i = 0; i < count; i++)
        {
            var name = LocationName.Create($"");
            if (name.IsFailure)
            {
                return Error.Failure("ErrorType.FAILURE", "Name is invalid");
            }

            var address = Address.Create(
                $"",
                $"",
                $"",
                $"",
                $"",
                $"");
            if (address.IsFailure)
            {
                return Error.Failure("ErrorType.FAILURE", "Address is invalid");
            }

            var timezone = Timezone.Create(string.Empty);
            if (timezone.IsFailure)
            {
                return Error.Failure("ErrorType.FAILURE", "Timezone is invalid");
            }

            var location = Location.Create(
                new LocationId(Guid.NewGuid()),
                name.Value,
                address.Value,
                timezone.Value,
                []);
            if (location.IsFailure)
            {
                return Error.Failure("ErrorType.FAILURE", "Location is invalid");
            }

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

    protected async Task<List<Guid>> CreateDepartmentsHierarchy(Guid[] locationIds)
{
    List<DepartmentLocation> CreateLocations(DepartmentId depId) =>
        locationIds.Select(id => new DepartmentLocation(
            new DepartmentLocationId(Guid.NewGuid()),
            depId,
            new LocationId(id)))
        .ToList();

    Department CreateDept(
        string name,
        string identifier,
        Department? parent = null)
    {
        var depId = new DepartmentId(Guid.NewGuid());
        var depLocations = CreateLocations(depId);

        var department = parent is null
            ? Department.CreateParent(
                DepartmentName.Create(name).Value,
                Identifier.Create(identifier).Value,
                depLocations,
                depId).Value
            : Department.CreateChild(
                DepartmentName.Create(name).Value,
                Identifier.Create(identifier).Value,
                parent,
                depLocations).Value;

        if (parent is not null)
            department.SetParent(parent);

        return department;
    }

    /*
     Headquarters
        ├─ Berlin Office
        │  ├─ Engineering Department
        │  ├─ Marketing Department
        │  ├─ Sales Department
        │  └─ Customer Support Center
        └─ Munich Office
    */

    var headquarters = CreateDept("Headquarters", "hq");
    var berlin = CreateDept("Berlin Office", "berlin", headquarters);
    var munich = CreateDept("Munich Office", "munich", headquarters);
    var engineering = CreateDept("Engineering Department", "engineering", berlin);
    var marketing = CreateDept("Marketing Department", "marketing", berlin);
    var sales = CreateDept("Sales Department", "sales", berlin);
    var support = CreateDept("Customer Support Center", "support", berlin);

    var all = new[]
    {
        headquarters, berlin, munich,
        engineering, marketing, sales, support
    };

    // Сохраняем в базу
    return await ExecuteInDb(async db =>
    {
        await db.Departments.AddRangeAsync(all);
        await db.SaveChangesAsync();

        return all.Select(d => d.Id.Value).ToList();
    });
}
}