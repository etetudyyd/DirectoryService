using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is has to be not empty.");
    }
}