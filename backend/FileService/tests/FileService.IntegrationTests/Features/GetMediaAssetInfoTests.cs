using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.Dtos;
using DirectoryService.HttpCommunication;
using DirectoryService.Infrastructure;
using DirectoryService.Requests;
using DirectoryService.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Shared.SharedKernel;

namespace DirectoryService.Features;

public class GetMediaAssetInfoTests : FileServiceBaseClass
{
    public GetMediaAssetInfoTests(FileServiceTestsWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetMediaAssetInfo_Should_Return_Data()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        MediaAsset createMediaAssetResponse = await CreateVideoAssetAsync(
            fileInfo,
            cancellationToken);

        // act
        Result<MediaAssetInfoDto?, Errors> getMediaAssetInfoResponse = await GetMediaAssetInfo(
            createMediaAssetResponse.Id,
            cancellationToken);

        // assert
        Assert.NotNull(fileInfo);
        Assert.True(getMediaAssetInfoResponse.IsSuccess);
        Assert.NotNull(getMediaAssetInfoResponse.Value);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == createMediaAssetResponse.Id, cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(createMediaAssetResponse.Id, mediaAsset.Id);
        });
    }

    [Fact]
    public async Task GetMediaAssetInfo_Should_Return_Data_With_Ready_Status()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        MediaAsset createMediaAssetResponse = await CreateVideoAssetReadyAsync(
            fileInfo,
            cancellationToken);

        // act
        Result<MediaAssetInfoDto?, Errors> getMediaAssetInfoResponse = await GetMediaAssetInfo(
            createMediaAssetResponse.Id,
            cancellationToken);

        // assert
        Assert.NotNull(fileInfo);
        Assert.True(getMediaAssetInfoResponse.IsSuccess);
        Assert.NotNull(getMediaAssetInfoResponse.Value);
        Assert.NotNull(getMediaAssetInfoResponse.Value.DownloadUrl);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == createMediaAssetResponse.Id, cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(createMediaAssetResponse.Id, mediaAsset.Id);
            Assert.Equal(MediaStatus.READY, mediaAsset.Status);
        });
    }

    [Fact]
    public async Task GetMediaAssetsInfo_Should_Return_Requested_MediaAssets_With_Ready_Status()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var fileInfo = new FileInfo(
            Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        var firstMediaAsset = await CreateVideoAssetReadyAsync(fileInfo, cancellationToken);
        var secondMediaAsset = await CreateVideoAssetReadyAsync(fileInfo, cancellationToken);

        var request = new GetMediaAssetsInfoRequest(
        [
            firstMediaAsset.Id,
            secondMediaAsset.Id
        ]);

        // Act
        var result = await GetMediaAssetsInfo(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var returnedAssets = result.Value.MediaAssets;

        Assert.Equal(2, returnedAssets.Count);

        var returnedIds = returnedAssets
            .Select(x => x.MediaAssetId)
            .ToHashSet();

        var returnedStatuses = returnedAssets
            .Select(m => m.Status)
            .ToHashSet();

        Assert.Contains(MediaStatus.READY.GetDisplayName(), returnedStatuses);
        Assert.Contains(firstMediaAsset.Id, returnedIds);
        Assert.Contains(secondMediaAsset.Id, returnedIds);
    }

    [Fact]
    public async Task GetMediaAssetsInfo_Should_Return_Requested_MediaAssets()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var fileInfo = new FileInfo(
            Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        var firstMediaAsset = await CreateVideoAssetAsync(fileInfo, cancellationToken);
        var secondMediaAsset = await CreateVideoAssetAsync(fileInfo, cancellationToken);

        var request = new GetMediaAssetsInfoRequest(
        [
            firstMediaAsset.Id,
            secondMediaAsset.Id
        ]);

        // Act
        var result = await GetMediaAssetsInfo(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var returnedAssets = result.Value.MediaAssets;

        Assert.Equal(2, returnedAssets.Count);

        var returnedIds = returnedAssets
            .Select(x => x.MediaAssetId)
            .ToHashSet();

        Assert.Contains(firstMediaAsset.Id, returnedIds);
        Assert.Contains(secondMediaAsset.Id, returnedIds);
    }

    [Fact]
    public async Task GetMediaAssetsInfo_Should_Return_Only_Existing_MediaAssets()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var fileInfo = new FileInfo(
            Path.Combine(AppContext.BaseDirectory, "Resources", Constants.TESTFILE_VIDEO));

        var existingAsset = await CreateVideoAssetAsync(fileInfo, cancellationToken);
        var missingId = Guid.NewGuid();

        var request = new GetMediaAssetsInfoRequest(
        [
            existingAsset.Id,
            missingId
        ]);

        // Act
        var result = await GetMediaAssetsInfo(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.MediaAssets);
        Assert.Equal(existingAsset.Id, result.Value.MediaAssets[0].MediaAssetId);
    }

    [Fact]
    public async Task GetMediaAssetInfo_Should_Return_Null()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        // act
        Result<MediaAssetInfoDto?, Errors> getMediaAssetInfoResponse = await GetMediaAssetInfo(
            Guid.NewGuid(),
            cancellationToken);

        // assert
        Assert.True(getMediaAssetInfoResponse.IsSuccess);
        Assert.Null(getMediaAssetInfoResponse.Value);
    }

    private async Task<Result<MediaAssetInfoDto?, Errors>> GetMediaAssetInfo(
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage getAssetInfoResponseMessage = await AppHttpClient
            .GetAsync($"/api/files/{mediaAssetId}", cancellationToken);

        return await getAssetInfoResponseMessage
            .HandleResponseAsync<MediaAssetInfoDto?>(cancellationToken);
    }

    private async Task<Result<GetMediaAssetsInfoResponse, Errors>> GetMediaAssetsInfo(
        GetMediaAssetsInfoRequest request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage getAssetsInfoResponseMessage = await AppHttpClient
            .PostAsJsonAsync($"/api/files/batch", request, cancellationToken);

        return await getAssetsInfoResponseMessage
            .HandleResponseAsync<GetMediaAssetsInfoResponse>(cancellationToken);
    }

}