using Core.Validation;
using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
        RuleForEach(x => x.DepartmentRequest.LocationsIds)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}