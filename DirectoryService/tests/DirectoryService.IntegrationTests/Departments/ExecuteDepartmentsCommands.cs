using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class ExecuteDepartmentsCommands(DirectoryTestWebFactory factory)
    : DirectoryBaseTests(factory)
{
    protected async Task<TResult> ExecuteCreateCommand<TResult>(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        await using var scope = Services.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<CreateDepartmentCommand>();

        return await handler.Handle(command, cancellationToken);
    }

    protected async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Location.Create(
                LocationName.Create("location_1").Value,
                Address.Create(
                    "123456",
                    "region",
                    "city",
                    "street",
                    "house").Value,
                Timezone.Create("Europe/London").Value,
                []).Value;

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }
}