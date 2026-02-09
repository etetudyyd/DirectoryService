using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.UpdatePositionDepartments;

public class UpdatePositionDepartmentsCommandValidator : AbstractValidator<UpdatePositionDepartmentsCommand>
{
    public UpdatePositionDepartmentsCommandValidator()
    {
        RuleFor(x => x.PositionId)
            .NotEmpty().WithMessage("Id is has to be not empty.");
        RuleForEach(x => x.PositionDepartmentsRequest.DepartmentsIds)
            .NotEmpty().WithMessage("To update position is has to be at least one department.");
    }
}