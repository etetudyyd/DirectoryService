using Core.Validation;
using DirectoryService.Types;
using DirectoryService.VOs;
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

        RuleFor(f => f.Request)
            .MustBeValueObject(r => MediaOwner.Create(r.Context, r.ContextId));

        RuleFor(f => f.Request.ContentType).MustBeValueObject(ContentType.Create);

        RuleFor(f => f.Request.FileName)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("FileName"));

        RuleFor(f => f.Request.Size)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("Size"));

        RuleFor(f => f.Request.AssetType)
            .Must(type => Enum.TryParse<AssetType>(type, true, out _))
            .WithError(GeneralErrors.General.ValueIsInvalid());
    }
}