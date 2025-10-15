using FluentValidation;

namespace DirectoryService.Application.CQRS.Departments.Commands.RelocateDepartmentParent;

public class RelocateDepartmentParentCommandValidator : AbstractValidator<RelocateDepartmentParentCommand>
{
    public RelocateDepartmentParentCommandValidator()
    {
    }
}