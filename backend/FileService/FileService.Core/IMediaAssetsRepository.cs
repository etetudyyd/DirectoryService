using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using Shared.SharedKernel;

namespace DirectoryService;

public interface IMediaAssetsRepository
{
    Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken);
}