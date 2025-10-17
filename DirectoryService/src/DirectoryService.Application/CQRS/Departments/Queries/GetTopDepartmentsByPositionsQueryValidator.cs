using FluentValidation;

namespace DirectoryService.Application.CQRS.Departments.Queries;

public class GetTopDepartmentsByPositionsQueryValidator : AbstractValidator<GetTopDepartmentsByPositionsQuery>
{
    public GetTopDepartmentsByPositionsQueryValidator() { }
}