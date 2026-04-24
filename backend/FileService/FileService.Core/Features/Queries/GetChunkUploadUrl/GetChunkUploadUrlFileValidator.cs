using Core.Validation;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetChunkUploadUrl;

public class GetChunkUploadUrlFileValidator : AbstractValidator<GetChunkUploadUrlFileQuery>
{
    public GetChunkUploadUrlFileValidator()
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

        RuleFor(f => f.Request.PartNumber)
            .NotNull()
            .WithError(GeneralErrors.General.ValueIsRequired("PartNumber"))
            .GreaterThan(0)
            .WithError(GeneralErrors.General.ValueIsInvalid("PartNumber must be greater than 0."));
    }
}