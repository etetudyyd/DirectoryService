using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Queries.GetChildrenDepartments;

public class GetChildrenDepartmentsQueryValidator : AbstractValidator<GetChildrenDepartmentsQuery>
{
    public GetChildrenDepartmentsQueryValidator()
    {
        RuleFor(x => x.Request.ParentId)
            .NotEmpty().WithMessage("ParentId is has to be not empty.");
    }
}