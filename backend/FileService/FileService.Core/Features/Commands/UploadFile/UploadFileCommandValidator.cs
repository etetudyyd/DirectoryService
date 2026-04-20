using Core.Validation;
using DirectoryService.Requests;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(f => f.Request)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsInvalid());

        RuleFor(f => f.Request)
            .MustBeValueObject(r => MediaOwner.Create(r.Context, r.ContextId));

        RuleFor(f => f.Request.FormFile)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired(nameof(UploadFileRequest.FormFile)));

        RuleFor(f => f.Request.FormFile.ContentType).MustBeValueObject(ContentType.Create);

        RuleFor(f => f.Request.FormFile.Length).GreaterThan(0)
            .WithError(GeneralErrors.General.ValueIsInvalid(nameof(UploadFileRequest.FormFile)));

        RuleFor(f => f.Request.AssetType)
            .Must(type => Enum.TryParse<AssetType>(type, true, out _))
            .WithError(GeneralErrors.General.ValueIsInvalid());
    }
}