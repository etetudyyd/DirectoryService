using FluentValidation;

namespace DirectoryService.Features.Departments.Commands.DeactivateDepartment;

public class DeactivateDepartmentCommandValidator : AbstractValidator<DeactivateDepartmentCommand>
{
    public DeactivateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is has to be not empty.");
    }
}