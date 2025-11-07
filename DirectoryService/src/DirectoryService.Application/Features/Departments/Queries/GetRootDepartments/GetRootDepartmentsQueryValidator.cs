using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Queries.GetRootDepartments;

public class GetRootDepartmentsQueryValidator : AbstractValidator<GetRootDepartmentsQuery>
{
    public GetRootDepartmentsQueryValidator()
    {
        RuleFor(x => x.DepartmentsRequest.Prefetch)
            .LessThan(x => x.DepartmentsRequest.PageSize);
    }
}