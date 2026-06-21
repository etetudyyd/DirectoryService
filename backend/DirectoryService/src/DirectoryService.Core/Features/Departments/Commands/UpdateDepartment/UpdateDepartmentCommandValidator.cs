using Core.Validation;
using DirectoryService.ValueObjects.Department;
using FluentValidation;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .MustBeValueObject(DepartmentName.Create);
        RuleFor(x => x.Request.Identifier)
            .MustBeValueObject(Identifier.Create);
    }
}