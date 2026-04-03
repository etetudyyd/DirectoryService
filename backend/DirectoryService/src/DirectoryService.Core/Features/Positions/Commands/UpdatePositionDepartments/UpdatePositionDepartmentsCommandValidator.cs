using Core.Validation;
using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.UpdatePositionDepartments;

public class UpdatePositionDepartmentsCommandValidator : AbstractValidator<UpdatePositionDepartmentsCommand>
{
    public UpdatePositionDepartmentsCommandValidator()
    {
        RuleFor(x => x.PositionId)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
        RuleForEach(x => x.PositionDepartmentsRequest.DepartmentsIds)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}