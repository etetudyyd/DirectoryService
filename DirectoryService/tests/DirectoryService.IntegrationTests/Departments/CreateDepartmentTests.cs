using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests(DirectoryTestWebFactory factory) : ExecuteDepartmentsCommands(factory)
{
    [Fact]
    public async Task CreateDepartment_With_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        var command = new CreateDepartmentCommand(
            new CreateDepartmentDto(
                "Department",
                "Test",
                null,
                [locationId.Value]));

        // act
        var result = await ExecuteCreateCommand(command, cancellationToken);

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }
}