using Core.Validation;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.AbortMultipartUpload;

public class AbortMultipartUploadFileValidator : AbstractValidator<AbortMultipartUploadFileCommand>
{
    public AbortMultipartUploadFileValidator()
    {
        RuleFor(f => f.Request.MediaAssetId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("MediaAssetId"));

        RuleFor(f => f.Request.UploadId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("UploadId"));
    }
}