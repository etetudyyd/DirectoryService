using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Extentions;
using DirectoryService.Application.IRepositories;
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

        var name = DepartmentName.Create(command.DepartmentDto.Name.Value);
        if (name.IsFailure)
        {
            _logger.LogError("Invalid DepartmentDto.Name");
            return name.Error.ToErrors();
        }

        var identifier = Identifier.Create(command.DepartmentDto.Identifier.Value);
        if (identifier.IsFailure)
        {
            _logger.LogError("Invalid DepartmentDto.Identifier");
            return name.Error.ToErrors();
        }

        var departmentId = new DepartmentId(Guid.NewGuid());

        var departmentLocationsList = command.DepartmentDto.DepartmentLocations
            .Select(dl => new DepartmentLocation
            {
                Id = new DepartmentLocationId(Guid.NewGuid()),
                DepartmentId = departmentId,
                LocationId = dl.LocationId,
            })
            .ToList();


        Result<Department, Error> department;

        if (command.DepartmentDto.ParentId is null)
        {
            department = Department.CreateParent(
                name.Value,
                identifier.Value,
                departmentLocationsList);
        }
        else
        {
            var parent = _departmentsRepository.GetAsync(command.DepartmentDto.ParentId.Value, cancellationToken).Result;
            department = Department.CreateChild(
                name.Value,
                identifier.Value,
                parent.Value,
                departmentLocationsList);
        }

        if (department.IsFailure)
        {
            _logger.LogError("Invalid DepartmentDto.Department");
            return department.Error.ToErrors();
        }

        await _departmentsRepository.AddAsync(department.Value, cancellationToken);

        _logger.LogInformation($"Department created successfully with id {departmentId}", departmentId);

        return departmentId.Value;
    }
}