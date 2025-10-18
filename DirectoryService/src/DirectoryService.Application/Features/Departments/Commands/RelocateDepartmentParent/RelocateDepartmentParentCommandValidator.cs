using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands.RelocateDepartmentParent;

public class RelocateDepartmentParentCommandValidator : AbstractValidator<RelocateDepartmentParentCommand>
{
    public RelocateDepartmentParentCommandValidator()
    {
    }
}