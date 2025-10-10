using FluentValidation;

namespace DirectoryService.Application.CQRS.Departments.Commands.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
    }
}