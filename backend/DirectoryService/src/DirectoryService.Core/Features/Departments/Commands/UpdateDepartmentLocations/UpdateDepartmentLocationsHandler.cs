using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsHandler : ICommandHandler<Guid, UpdateDepartmentLocationsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ILocationsRepository _locationsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdateDepartmentLocationsCommandValidator _validator;

    private readonly ILogger<UpdateDepartmentLocationsHandler> _logger;
    
    private readonly HybridCache _cache;

    public UpdateDepartmentLocationsHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        UpdateDepartmentLocationsCommandValidator validator,
        ILogger<UpdateDepartmentLocationsHandler> logger, HybridCache cache)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid DepartmentDto");
            return validationResult.ToErrors();
        }

        var departmentId = new DepartmentId(command.DepartmentId);

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        var department = await _departmentsRepository
            .GetByIdAsync(departmentId.Value, cancellationToken);

        // isExists validation
        if (department.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return department.Error.ToErrors();
        }

        var locationIds = department.Value.DepartmentLocations
            .Select(x => x.LocationId)
            .ToArray();

        var departmentLocationsResult = await _locationsRepository
            .IsLocationActiveAsync(locationIds, cancellationToken);

        var updatedLocations = command.DepartmentRequest.LocationsIds
            .Select(ul => new LocationId(ul))
            .ToArray();

        var departmentLocationsUpdateResult = await _locationsRepository
            .IsLocationActiveAsync(updatedLocations, cancellationToken); 


        // Locations isExists, isActive, isUnique validation
        if (departmentLocationsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return departmentLocationsResult.Error.ToErrors();
        }

        if (departmentLocationsUpdateResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return departmentLocationsUpdateResult.Error.ToErrors();
        }

        List<DepartmentLocation> departmentLocations = [];

        foreach (var departmentLocationsRequest in command.DepartmentRequest.LocationsIds)
        {
            var departmentLocation = DepartmentLocation.Create(
                new LocationId(departmentLocationsRequest),
                departmentId);

            if (departmentLocation.IsFailure)
            {
                transactionScope.Rollback(cancellationToken);
                return departmentLocation.Error.ToErrors();
            }

            departmentLocations.Add(departmentLocation.Value);
        }

        department.Value.UpdateLocations(departmentLocations);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        var commitedResult = transactionScope.Commit(cancellationToken);
        if (commitedResult.IsFailure)
        {
            return commitedResult.Error.ToErrors();
        }

        await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Department was updated with id{departmentId.Value}");

        return departmentId.Value;
    }
}