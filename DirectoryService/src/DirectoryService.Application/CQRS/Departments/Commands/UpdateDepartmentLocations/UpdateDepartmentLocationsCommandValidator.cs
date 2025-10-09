using FluentValidation;

namespace DirectoryService.Application.Features.Departments.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
    }
}