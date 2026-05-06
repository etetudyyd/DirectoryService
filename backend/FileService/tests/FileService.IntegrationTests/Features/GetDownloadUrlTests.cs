using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.HttpCommunication;
using DirectoryService.Infrastructure;
using Shared.SharedKernel;

namespace DirectoryService.Features;

public class GetDownloadUrlTests : FileServiceBaseClass
{
    public GetDownloadUrlTests(FileServiceTestsWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetDownloadUrl_Should_Be_Success()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        MediaAsset createMediaAssetResponse = await CreateVideoAssetAsync(fileInfo, cancellationToken);

        // act
        UnitResult<Errors> getDownloadUrlResponse = await GetDownloadUrl(
            createMediaAssetResponse.RawKey.FullPath,
            cancellationToken);

        // assert
        Assert.NotNull(fileInfo);
        Assert.True(getDownloadUrlResponse.IsSuccess);
    }

    private async Task<UnitResult<Errors>> GetDownloadUrl(
        string path,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage getDownloadUrlResponseMessage = await AppHttpClient
            .GetAsync($"/api/files/download/url/{path}", cancellationToken);

        return await getDownloadUrlResponseMessage
            .HandleResponseAsync(cancellationToken);
    }
}