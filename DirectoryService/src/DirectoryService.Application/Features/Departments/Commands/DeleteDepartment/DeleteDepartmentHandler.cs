using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Extentions;
using DirectoryService.Application.Features.Departments.Commands.CreateDepartment;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentHandler : ICommandHandler<Guid, DeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ILogger<DeleteDepartmentHandler> _logger;

    private readonly IValidator<DeleteDepartmentCommand> _validator;

    public DeleteDepartmentHandler(
        ILogger<DeleteDepartmentHandler> logger,
        IValidator<DeleteDepartmentCommand> validator,
        IDepartmentsRepository departmentsRepository)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeleteDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid DepartmentDto");
            return validationResult.ToErrors();
        }
        
        
    }
}