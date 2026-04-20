using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Repositories;

public class MediaAssetsRepository : IMediaAssetsRepository
{
    private readonly FileServiceDbContext _dbContext;
    private readonly ILogger<MediaAssetsRepository> _logger;

    public MediaAssetsRepository(FileServiceDbContext dbContext, ILogger<MediaAssetsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.MediaAssets.AddAsync(mediaAsset, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return mediaAsset.Id;
        }
        catch (Exception ex)
        {
            return GeneralErrors.General.ValueIsInvalid(ex.Message);
        }
    }

    public async Task<Result<MediaAsset, Error>> GetBy(Expression<Func<MediaAsset, bool>> predicate, CancellationToken cancellationToken = default)
    {
        MediaAsset? mediaAsset = await _dbContext.MediaAssets.FirstOrDefaultAsync(predicate, cancellationToken);

        if (mediaAsset is null)
            return GeneralErrors.General.NotFound();

        return mediaAsset;
    }

    public async Task<UnitResult<Error>> RemoveAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.MediaAssets.Remove(mediaAsset);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove mediaAsset. {ex}", ex);
            return GeneralErrors.General.ValueIsInvalid();
        }

        return UnitResult.Success<Error>();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to save changes of mediaAsset. {ex}", ex);
        }
    }
}