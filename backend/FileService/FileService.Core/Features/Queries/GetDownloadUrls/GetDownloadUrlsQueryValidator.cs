using Core.Validation;
using DirectoryService.FilesStorage;
using DirectoryService.VOs;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Queries.GetDownloadUrls;

public class GetDownloadUrlsQueryValidator : AbstractValidator<GetDownloadUrlsQuery>
{
    public GetDownloadUrlsQueryValidator()
    {
        RuleForEach(x => x.Paths)
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