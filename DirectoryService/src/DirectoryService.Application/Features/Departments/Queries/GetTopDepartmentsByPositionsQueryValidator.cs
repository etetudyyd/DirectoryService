using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Queries;

public class GetTopDepartmentsByPositionsQueryValidator : AbstractValidator<GetTopDepartmentsByPositionsQuery>
{
    public GetTopDepartmentsByPositionsQueryValidator() { }
}