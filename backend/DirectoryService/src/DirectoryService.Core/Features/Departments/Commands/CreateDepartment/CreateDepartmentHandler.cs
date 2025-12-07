using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ILogger<CreateDepartmentHandler> _logger;

    private readonly IValidator<CreateDepartmentCommand> _validator;
    
    private readonly HybridCache _cache;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<CreateDepartmentHandler> logger,
        IValidator<CreateDepartmentCommand> validator, HybridCache cache)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
        _validator = validator;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
    CreateDepartmentCommand command,
    CancellationToken cancellationToken)
{
    var validationResult = await _validator.ValidateAsync(command, cancellationToken);
    if (!validationResult.IsValid)
    {
        _logger.LogError("Invalid DepartmentDto");
        return validationResult.ToErrors();
    }

    var nameResult = DepartmentName.Create(command.DepartmentRequest.Name);
    if (nameResult.IsFailure)
    {
        _logger.LogError("Invalid DepartmentDto.Name");
        return nameResult.Error.ToErrors();
    }

    var identifierResult = Identifier.Create(command.DepartmentRequest.Identifier);
    if (identifierResult.IsFailure)
    {
        _logger.LogError("Invalid DepartmentDto.Identifier");
        return identifierResult.Error.ToErrors();
    }

    var departmentId = new DepartmentId(Guid.NewGuid());

    var departmentLocations = command.DepartmentRequest.LocationsIds
        .Select(locationId => new DepartmentLocation(
            new DepartmentLocationId(Guid.NewGuid()),
            departmentId,
            new LocationId(locationId)))
        .ToList();



    Result<Department, Error> departmentResult;
    if (command.DepartmentRequest.ParentId is null)
    {
        departmentResult = Department.CreateParent(
            nameResult.Value,
            identifierResult.Value,
            departmentLocations,
            departmentId);
    }
    else
    {
        var parentResult = await _departmentsRepository
            .GetByIdAsync(command.DepartmentRequest.ParentId.Value, cancellationToken);
        if (parentResult.IsFailure)
        {
            _logger.LogError("Parent department not found");
            return parentResult.Error.ToErrors();
        }

        departmentResult = Department.CreateChild(
            nameResult.Value,
            identifierResult.Value,
            parentResult.Value,
            departmentLocations,
            departmentId);
    }

    if (departmentResult.IsFailure)
    {
        _logger.LogError("Invalid DepartmentDto.Department");
        return departmentResult.Error.ToErrors();
    }

    await _departmentsRepository.AddAsync(departmentResult.Value, cancellationToken);

    await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

    _logger.LogInformation("Department created successfully with id {departmentId}", departmentId);

    return departmentId.Value;
}

}