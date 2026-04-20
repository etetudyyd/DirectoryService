using Core.Validation;
using DirectoryService.Validators;
using DirectoryService.ValueObjects.Department;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentRequest.Name)
            .MustBeValueObject(DepartmentName.Create);
        RuleFor(x => x.DepartmentRequest.Identifier)
            .MustBeValueObject(Identifier.Create);
        RuleForEach(x => x.DepartmentRequest.LocationsIds)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}