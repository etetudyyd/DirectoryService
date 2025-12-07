using FluentValidation;

namespace DirectoryService.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentRequest.Name)
            .MinimumLength(Constants.MIN_LENGTH_DEPARTMENT_NAME).WithMessage("Name is has to be at least 3 characters long")
            .MaximumLength(Constants.MAX_LENGTH_DEPARTMENT_NAME).WithMessage("Name is has to be at most 100 characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");
        RuleFor(x => x.DepartmentRequest.Identifier)
            .MinimumLength(Constants.MIN_LENGTH_DEPARTMENT_IDENTIFIER)
            .WithMessage($"Identifier is has to be at least {Constants.MIN_LENGTH_DEPARTMENT_IDENTIFIER} characters long")
            .MaximumLength(Constants.MAX_LENGTH_DEPARTMENT_IDENTIFIER).WithMessage($"Identifier is has to be at most {Constants.MAX_LENGTH_DEPARTMENT_IDENTIFIER} characters long")
            .NotEmpty().WithMessage("Identifier is has to be not empty");
        RuleForEach(x => x.DepartmentRequest.LocationsIds)
            .NotEmpty().WithMessage("Department is has to be at least 1 location.");
    }
}