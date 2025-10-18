using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
    }
}