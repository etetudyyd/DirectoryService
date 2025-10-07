using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Features.Departments.UpdateDepartmentLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentLocationsTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task UpdateDepartmentLocations_With_Valid_Data_Should_Succeed()
    {
        var locationIds = await CreateNLocationsValid(1);
        var departmentId = await CreateDepartmentParentValid(locationIds);

        var locationUpdateIds = await CreateNLocationsToUpdateValid(1);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId,
                new UpdateDepartmentLocationsDto(
                    locationUpdateIds));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotNull(department);
            Assert.NotEqual(Guid.Empty, result.Value);

            Assert.Equal(result.Value, department.Id.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentLocations_With_Invalid_Data_Should_Failed()
    {
        var cancellationToken = CancellationToken.None;
        var locationIds = await CreateNLocationsValid(1);
        var departmentId = await CreateDepartmentParentValid(locationIds);

        var locationUpdateIds = await CreateNLocationsInvalid(1);

        if (locationUpdateIds.IsSuccess)
        {
            var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Error>>(async sut =>
            {
                var command = new UpdateDepartmentLocationsCommand(
                    departmentId,
                    new UpdateDepartmentLocationsDto(
                        locationUpdateIds.Value));

                var result = await sut.Handle(command, cancellationToken);
                return result.Value;
            });

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.FAILURE, result.Error.Type);

            Assert.NotEqual(locationIds, locationUpdateIds);
        }

        // assert
        Assert.True(locationUpdateIds.IsFailure);
        Assert.Equal(ErrorType.FAILURE, locationUpdateIds.Error.Type);
    }

    [Fact]
    public async Task UpdateDepartmentLocations_With_Department_Doesnt_Exist_Should_Failed()
    {
        var cancellationToken = CancellationToken.None;

        var locationUpdateIds = await CreateNLocationsToUpdateValid(1);

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Error>>(async sut =>
            {
                var command = new UpdateDepartmentLocationsCommand(
                    Guid.NewGuid(),
                    new UpdateDepartmentLocationsDto(
                        locationUpdateIds));

                var result = await sut.Handle(command, cancellationToken);
                if (result.IsFailure)
                {
                    return Error.Failure("ErrorType.FAILURE", "Could not update department");
                }

                return result.Value;
            });

        // assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }

    [Fact]
    public async Task UpdateDepartmentLocations_With_Location_Doesnt_Exist_Should_Failed()
    {
        var cancellationToken = CancellationToken.None;
        var locationIds = await CreateNLocationsValid(1);
        var departmentId = await CreateDepartmentParentValid(locationIds);

        var locationUpdateIds = new[] { Guid.NewGuid() };

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Error>>(async sut =>
            {
                var command = new UpdateDepartmentLocationsCommand(
                    departmentId,
                    new UpdateDepartmentLocationsDto(
                        locationUpdateIds));

                var result = await sut.Handle(command, cancellationToken);
                if (result.IsFailure)
                {
                    return Error.Failure("ErrorType.FAILURE", "Could not update department");
                }

                return result.Value;
            });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);

        Assert.NotEqual(locationIds, locationUpdateIds);
    }
}