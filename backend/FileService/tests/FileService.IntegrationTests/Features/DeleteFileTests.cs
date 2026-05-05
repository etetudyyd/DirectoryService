using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.HttpCommunication;
using DirectoryService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.SharedKernel;

namespace DirectoryService.Features;

public class DeleteFileTests : FileServiceBaseClass
{
    public DeleteFileTests(FileServiceTestsWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteFile_Should_Be_Success()
    {
        // arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        MediaAsset createMediaAssetResponse = await CreateVideoAssetAsync(fileInfo, cancellationToken);

        // act
        UnitResult<Errors> deleteFileResponse = await DeleteFile(createMediaAssetResponse.Id, cancellationToken);

        // assert
        Assert.True(fileInfo.Exists);
        Assert.True(deleteFileResponse.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == createMediaAssetResponse.Id, cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.DELETED, mediaAsset.Status);
        });

    }

    [Fact]
    public async Task DeleteFile_Should_Be_Failure_Invalid_Request()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        MediaAsset createMediaAssetResponse = await CreateVideoAssetAsync(fileInfo, cancellationToken);

        // act
        UnitResult<Errors> deleteFileResponse = await DeleteFile(Guid.NewGuid(), cancellationToken);

        // assert
        Assert.True(fileInfo.Exists);
        Assert.True(deleteFileResponse.IsFailure);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == createMediaAssetResponse.Id, cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.NotEqual(MediaStatus.DELETED, mediaAsset.Status);
        });

    }

    private async Task<UnitResult<Errors>> DeleteFile(
        Guid id,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage deleteResponseMessage = await AppHttpClient
            .DeleteAsync($"/api/files/delete/{id}", cancellationToken);

        return await deleteResponseMessage
            .HandleResponseAsync(cancellationToken);
    }
}