using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Features.Departments.Commands.DeleteInactiveDepartments;
using DirectoryService.IntegrationTests.Infrastructure;

namespace DirectoryService.IntegrationTests.Departments;

public class DeleteInactiveDepartmentsTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task DeleteInactiveDepartments_With_Valid_Data_Should_Succeed()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchyWithInactive(locationIds);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<DeleteInactiveDepartmentsHandler, UnitResult<Errors>>(async sut =>
            await sut.Handle(new DeleteInactiveDepartmentsCommand(), cancellationToken));

        Assert.True(result.IsSuccess);

    }

}