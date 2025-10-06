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

public class CreateDepartmentTests(DirectoryTestWebFactory factory)
    : DirectoryBaseTests(factory)
{
    private new IServiceProvider Services { get; } = factory.Services;

    [Fact]
    public async Task CreateOneDepartment_With_One_Location_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationIds = await CreateNLocationsValid(1);
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentDto(
                    "DepartmentName",
                    "department-test",
                    null,
                    locationIds));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.Single(department?.DepartmentLocations);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartments_With_Three_LocationsValid_Data_Should_Succeed()
    {
        // arrange
        var locationIds = await CreateNLocationsValid(3);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentDto(
                    "DepartmentName",
                    "department-test",
                    null,
                    locationIds));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.Equal(3, department?.DepartmentLocations.Count);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateOneDepartment_With_One_Location_Invalid_Data_Should_Failed()
    {
        // arrange
        var locationIds = await CreateNLocationsValid(1);
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentDto(
                    string.Empty,
                    string.Empty,
                    null,
                    []));

            var result = await sut.Handle(command, cancellationToken);

            if (result.IsFailure)
                return Error.Failure("ErrorType.FAILURE", "Department not found");

            return result.Value;
        });

        Assert.True(result.IsFailure);

        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }

    [Fact]
    public async Task CreateDepartment_With_Parent_Doesnt_Exist_Should_Failed()
    {
        // arrange
        var locationIds = await CreateNLocationsValid(1);
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentDto(
                    "DepartmentName",
                    "department-test",
                    Guid.NewGuid(),
                    locationIds));

            var result = await sut.Handle(command, cancellationToken);
            if (result.IsFailure)
                return Error.Failure("ErrorType.FAILURE", "Department not found");

            return result.Value;
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }
}