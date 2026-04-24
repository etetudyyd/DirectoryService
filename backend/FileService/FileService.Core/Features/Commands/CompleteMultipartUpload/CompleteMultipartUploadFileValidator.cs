using Core.Validation;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.CompleteMultipartUpload;

public class CompleteMultipartUploadFileValidator : AbstractValidator<CompleteMultipartUploadFileCommand>
{
    public CompleteMultipartUploadFileValidator()
    {
        RuleFor(f => f.Request)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsInvalid());

        RuleFor(f => f.Request.MediaAssetId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("MediaAssetId"));

        RuleFor(f => f.Request.UploadId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("UploadId"));

        RuleForEach(f => f.Request.PartETags)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("PartETags"));
    }
}