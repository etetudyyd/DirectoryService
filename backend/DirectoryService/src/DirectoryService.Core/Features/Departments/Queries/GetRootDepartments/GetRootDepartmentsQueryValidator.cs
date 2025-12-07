using FluentValidation;

namespace DirectoryService.Features.Departments.Queries.GetRootDepartments;

public class GetRootDepartmentsQueryValidator : AbstractValidator<GetRootDepartmentsQuery>
{
    public GetRootDepartmentsQueryValidator()
    {
        RuleFor(x => x.Request.Prefetch)
            .LessThan(x => x.Request.PageSize);
    }
}