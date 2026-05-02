using DirectoryService.Assets;

namespace DirectoryService;

public interface IReadDbContext
{
    IQueryable<MediaAsset> MediaAssetsRead { get; }

}