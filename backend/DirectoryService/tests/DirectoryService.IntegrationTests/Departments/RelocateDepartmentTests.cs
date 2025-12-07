using CSharpFunctionalExtensions;
using DirectoryService.Departments.Requests;
using DirectoryService.Features.Departments.Commands.RelocateDepartmentParent;
using DirectoryService.Infrastructure;
using DirectoryService.ValueObjects.Department;
using Microsoft.EntityFrameworkCore;
using Shared.SharedKernel;

namespace DirectoryService.Departments;

public class RelocateDepartmentTests(DirectoryTestWebFactory factory)
    : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task RelocateDepartment_With_Valid_Data_Should_Succeed()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = departmentsIds[4];

        var newParentId = departmentsIds[3];

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotNull(department);

            Assert.Equal(department.ParentId?.Value, newParentId);
            Assert.Equal(result.Value, department.Id.Value);
        });
    }

    [Fact]
    public async Task RelocateDepartment_To_Another_Node_Should_Success()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = departmentsIds[3];

        var newParentId = departmentsIds[2];

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotNull(department);

            Assert.Equal(department.ParentId?.Value, newParentId);
            Assert.Equal(result.Value, department.Id.Value);
        });
    }

    [Fact]
    public async Task RelocateDepartment_With_Invalid_Data_Should_Failed()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = Guid.Empty;

        var newParentId = departmentsIds[0];

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return Error.Failure("department.not.found", "Department not found");
            }

            return result.Value;
        });

        // assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }

    [Fact]
    public async Task RelocateDepartment_Parent_Department_Doesnt_Exist_Should_Failed()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = departmentsIds[6];

        var newParentId = Guid.NewGuid();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return Error.Failure("department.not.found", "Department not found");
            }

            return result.Value;
        });

        // assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }

    [Fact]
    public async Task RelocateDepartment_To_Self_Child_Should_Failed()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = departmentsIds[1];

        var newParentId = departmentsIds[3];

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return Error.Failure("department.not.found", "Department not found");
            }

            return result.Value;
        });

        // assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.FAILURE, result.Error.Type);
    }

    [Fact]
    public async Task RelocateDepartment_To_Root_Should_Success()
    {
        var locationIds = await CreateNLocationsValid(1);

        var departmentsIds = await CreateDepartmentsHierarchy(locationIds);

        var child = departmentsIds[6];

        Guid? newParentId = null;

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<RelocateDepartmentParentHandler, Result<Guid, Error>>(async sut =>
        {
            var command = new RelocateDepartmentParentCommand(
                child,
                new RelocateDepartmentParentRequest(newParentId));

            var result = await sut.Handle(command, cancellationToken);
            return result.Value;
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotNull(department);

            Assert.Equal(department.ParentId?.Value, newParentId);
            Assert.Equal(result.Value, department.Id.Value);
        });
    }
}