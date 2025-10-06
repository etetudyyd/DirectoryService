using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.Infrastructure.Postgresql.Database;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task CreateDepartment_With_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationId = await CreateOneLocation();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentDto(
                    "DepartmentName",
                    "department-test",
                    null,
                    [locationId.Value]));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }
}