using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetRootDepartments;

public class GetRootDepartmentsQueryValidator : AbstractValidator<GetRootDepartmentsQuery>
{
    public GetRootDepartmentsQueryValidator()
    {
        RuleFor(x => x.Request.Prefetch)
            .LessThan(x => x.Request.PageSize)
            .WithError(GeneralErrors.General.ValueIsInvalid());
    }
}