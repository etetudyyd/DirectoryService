using FluentValidation;

namespace DirectoryService.Application.CQRS.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        
    }
}