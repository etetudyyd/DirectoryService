using FluentValidation;

namespace DirectoryService.Application.Features.Departments.RelocateDepartmentParent;

public class RelocateDepartmentParentCommandValidator : AbstractValidator<RelocateDepartmentParentCommand>
{
    public RelocateDepartmentParentCommandValidator()
    {
    }
}