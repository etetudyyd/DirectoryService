using FluentValidation;

namespace DirectoryService.Application.Features.Positions.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        
    }
}