using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.DeactivateDepartment;

public class DeactivateDepartmentCommandValidator : AbstractValidator<DeactivateDepartmentCommand>
{
    public DeactivateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}