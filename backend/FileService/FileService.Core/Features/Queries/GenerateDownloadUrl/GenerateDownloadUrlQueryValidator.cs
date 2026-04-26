using Core.Validation;
using DirectoryService.FilesStorage;
using DirectoryService.VOs;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GenerateDownloadUrl;

public class GenerateDownloadUrlQueryValidator : AbstractValidator<GenerateDownloadUrlQuery>
{
    public GenerateDownloadUrlQueryValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty()
            .WithError(GeneralErrors.General.ValueIsRequired("Path is required"))
            .Must(StorageKeyParser.BeValidPathStructure)
            .WithError(GeneralErrors.General.ValueIsInvalid("Path must have at least 2 parts separated by slashes"))
            .MustBeValueObject(p =>
            {
                (string location, string? prefix, string key) = StorageKeyParser.TryParse(p);
                return StorageKey.Create(location, prefix, key);
            });
    }
}