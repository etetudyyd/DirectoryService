using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetChildrenDepartments;

public class GetChildrenDepartmentsQueryValidator : AbstractValidator<GetChildrenDepartmentsQuery>
{
    public GetChildrenDepartmentsQueryValidator()
    {
        RuleFor(x => x.Request.ParentId)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}