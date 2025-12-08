using FluentValidation;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Id is has to be not empty.");
        RuleForEach(x => x.DepartmentRequest.LocationsIds)
            .NotEmpty().WithMessage("To update department is has to be at least one location.");
    }
}