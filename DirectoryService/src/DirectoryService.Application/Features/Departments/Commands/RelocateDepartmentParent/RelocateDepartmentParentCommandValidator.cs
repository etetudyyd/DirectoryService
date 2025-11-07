using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands.RelocateDepartmentParent;

public class RelocateDepartmentParentCommandValidator : AbstractValidator<RelocateDepartmentParentCommand>
{
    public RelocateDepartmentParentCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Id is has to be not empty.")
            .NotNull().WithMessage("Id is has to be null");
        RuleFor(x => x.DepartmentRequest.ParentId)
            .NotEmpty().WithMessage("ParentId is has to be not empty.");
    }
}