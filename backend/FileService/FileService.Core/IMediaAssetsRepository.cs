using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using Shared.SharedKernel;

namespace DirectoryService;

public interface IMediaAssetsRepository
{
    Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken);

    Task<Result<MediaAsset, Error>> GetBy(Expression<Func<MediaAsset, bool>> predicate, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> RemoveAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}