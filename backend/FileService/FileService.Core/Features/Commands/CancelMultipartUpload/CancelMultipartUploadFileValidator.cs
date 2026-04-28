using Core.Validation;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.CancelMultipartUpload;

public class CancelMultipartUploadFileValidator : AbstractValidator<CancelMultipartUploadFileCommand>
{
    public CancelMultipartUploadFileValidator()
    {
        RuleFor(f => f.Request.MediaAssetId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("MediaAssetId"));

        RuleFor(f => f.Request.UploadId)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("UploadId"));
    }
}