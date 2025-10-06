using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Extentions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.CreateDepartment;

public class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ILogger<CreateDepartmentHandler> _logger;

    private readonly IValidator<CreateDepartmentCommand> _validator;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<CreateDepartmentHandler> logger,
        IValidator<CreateDepartmentCommand> validator)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
        _validator = validator;
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

    var nameResult = DepartmentName.Create(command.DepartmentDto.Name);
    if (nameResult.IsFailure)
    {
        _logger.LogError("Invalid DepartmentDto.Name");
        return nameResult.Error.ToErrors();
    }

    var identifierResult = Identifier.Create(command.DepartmentDto.Identifier);
    if (identifierResult.IsFailure)
    {
        _logger.LogError("Invalid DepartmentDto.Identifier");
        return identifierResult.Error.ToErrors();
    }

    var departmentId = new DepartmentId(Guid.NewGuid());

    var departmentLocations = command.DepartmentDto.LocationsIds
        .Select(locationId => new DepartmentLocation(
            new DepartmentLocationId(Guid.NewGuid()),
            departmentId,
            new LocationId(locationId)))
        .ToList();



    Result<Department, Error> departmentResult;
    if (command.DepartmentDto.ParentId is null)
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
            .GetByIdAsync(command.DepartmentDto.ParentId.Value, cancellationToken);
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

    _logger.LogInformation("Department created successfully with id {departmentId}", departmentId);

    return departmentId.Value;
}

}