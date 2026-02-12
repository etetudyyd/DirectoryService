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

        var department = await _departmentsRepository
            .GetBy(x => x.Id == departmentId, cancellationToken);

        // isExists validation
        if (department.IsFailure)
        {
            _logger.LogError("Department is not active!");
            return department.Error.ToErrors();
        }

        var activeLocationIdsResult = await _locationsRepository
            .GetActiveLocationIdsAsync(command.DepartmentRequest.LocationsIds, cancellationToken);

        if (activeLocationIdsResult.IsFailure)
            return activeLocationIdsResult.Error.ToErrors();

        var bulkDeleteDepartmentLocationsResult = await _departmentsRepository
            .BulkDeleteDepartmentLocationsAsync(departmentId.Value, cancellationToken);

        if (bulkDeleteDepartmentLocationsResult.IsFailure)
        {
            _logger.LogError("Position departments wasn't updated!");
            return bulkDeleteDepartmentLocationsResult.Error.ToErrors();
        }

        List<DepartmentLocation> departmentLocations = [];

        foreach (var departmentLocationsRequest in command.DepartmentRequest.LocationsIds)
        {
            var departmentLocation = DepartmentLocation.Create(
                new LocationId(departmentLocationsRequest),
                departmentId);

            if (departmentLocation.IsFailure)
            {
                _logger.LogError("Department location wasn't updated!");
                return departmentLocation.Error.ToErrors();
            }

            departmentLocations.Add(departmentLocation.Value);
        }

        department.Value.UpdateLocations(departmentLocations);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Department was updated with id{departmentId.Value}");

        return departmentId.Value;
    }
}