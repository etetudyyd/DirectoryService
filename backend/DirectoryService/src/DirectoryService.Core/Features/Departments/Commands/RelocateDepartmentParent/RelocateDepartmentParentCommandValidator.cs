using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.RelocateDepartmentParent;

public class RelocateDepartmentParentCommandValidator : AbstractValidator<RelocateDepartmentParentCommand>
{
    public RelocateDepartmentParentCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired())
            .NotNull().WithError(GeneralErrors.General.ValueIsRequired());
    }
}