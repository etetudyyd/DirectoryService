using Core.Validation;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.StartMultipartUpload;

public class StartMultipartUploadFileValidator : AbstractValidator<StartMultipartUploadFileCommand>
{
    public StartMultipartUploadFileValidator()
    {
        RuleFor(f => f.Request)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsInvalid());
        
        
    }
}