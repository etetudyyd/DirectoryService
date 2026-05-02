using System.Net.Http.Json;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.HttpCommunication;
using DirectoryService.Infrastructure;
using DirectoryService.Requests;
using DirectoryService.Responses;
using Microsoft.EntityFrameworkCore;
using Shared.SharedKernel;

namespace DirectoryService.Features;

public class MultipartUploadFileTests : FileServiceBaseClass
{
    public MultipartUploadFileTests(FileServiceTestsWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UploadChunks_Should_Be_Success()
    {
        const int requestedPartNumber = 1;

        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        Result<GetChunkUploadUrlFileResponse, Errors> result = await GetUploadChunkUrlAsync(startMultipartUploadFileResponse, requestedPartNumber, cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.UploadUrl);
        Assert.Equal(requestedPartNumber, result.Value.PartNumber);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadFileResponse.MediaAssetId, cancellationToken);

            Assert.Equal(MediaStatus.UPLOADING, mediaAsset?.Status);
            Assert.NotNull(mediaAsset);
        });
    }

    [Fact]
    public async Task UploadChunks_Should_Be_Failure()
    {
        const int requestedPartNumber = 10;

        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        Result<GetChunkUploadUrlFileResponse, Errors> result = await GetUploadChunkUrlAsync(startMultipartUploadFileResponse, requestedPartNumber, cancellationToken);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadFileResponse.MediaAssetId, cancellationToken);

            Assert.Equal(MediaStatus.UPLOADING, mediaAsset?.Status);
            Assert.NotNull(mediaAsset);
        });
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsMediaFile()
    {
        // arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        IReadOnlyList<PartETagDto> partETagDtoS = await UploadChunksAsync(startMultipartUploadFileResponse, fileInfo, cancellationToken);

        UnitResult<Errors> result = await CompleteMultipartUpload(startMultipartUploadFileResponse, partETagDtoS, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadFileResponse.MediaAssetId, cancellationToken);

            Assert.Equal(MediaStatus.UPLOADED, mediaAsset?.Status);
            Assert.NotNull(mediaAsset);

            await ExecuteInS3Client(async s3Client =>
            {
                var objectRequest = await s3Client.GetObjectAsync(
                    mediaAsset.RawKey.Bucket,
                    mediaAsset.RawKey.Key,
                    cancellationToken);

                Assert.Equal(mediaAsset.MediaData.Size, objectRequest.ContentLength);
                Assert.Equal(mediaAsset.RawKey.Key, objectRequest.Key);
            });
        });
    }

    [Fact]
    public async Task MultipartUpload_Abort_PersistsMediaFile()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        UnitResult<Errors> abortMultipartUploadFileResponse = await AbortMultipartUpload(startMultipartUploadFileResponse, cancellationToken);

        // assert
        Assert.True(fileInfo.Exists);
        Assert.True(abortMultipartUploadFileResponse.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadFileResponse.MediaAssetId, cancellationToken);

            Assert.Equal(MediaStatus.FAILED, mediaAsset?.Status);
            Assert.NotNull(mediaAsset);
        });
    }

    [Fact]
    public async Task MultipartUpload_Cancel_PersistsMediaFile()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        UnitResult<Errors> cancelMultipartUploadFileResponse = await CancelMultipartUpload(startMultipartUploadFileResponse, cancellationToken);

        // assert
        Assert.True(fileInfo.Exists);
        Assert.True(cancelMultipartUploadFileResponse.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadFileResponse.MediaAssetId, cancellationToken);

            Assert.Null(mediaAsset);
        });
    }

    private async Task<UnitResult<Errors>> CompleteMultipartUpload(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        IReadOnlyList<PartETagDto> partETagDtoS,
        CancellationToken cancellationToken)
    {
        CompleteMultipartUploadFileRequest request = new(
            startMultipartUploadFileResponse.MediaAssetId,
            startMultipartUploadFileResponse.UploadId,
            partETagDtoS);

        HttpResponseMessage completeMultipartResponseMessage = await AppHttpClient
            .PostAsJsonAsync("/api/files/multipart/complete", request, cancellationToken);

        return await completeMultipartResponseMessage
            .HandleResponseAsync(cancellationToken);
    }

    private async Task<Result<GetChunkUploadUrlFileResponse, Errors>> GetUploadChunkUrlAsync(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        int requestedPartNumber,
        CancellationToken cancellationToken)
    {
        var request = new GetChunkUploadUrlFileRequest(
            startMultipartUploadFileResponse.MediaAssetId,
            startMultipartUploadFileResponse.UploadId,
            requestedPartNumber);

        HttpResponseMessage getChunkUrlResponseMessage = await AppHttpClient
            .PostAsJsonAsync("/api/files/multipart/url", request, cancellationToken);

        Result<GetChunkUploadUrlFileResponse, Errors> response = await getChunkUrlResponseMessage
            .HandleResponseAsync<GetChunkUploadUrlFileResponse>(cancellationToken);
        return response;
    }

    private async Task<UnitResult<Errors>> AbortMultipartUpload(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        CancellationToken cancellationToken)
    {
        AbortMultipartUploadFileRequest request = new(
            startMultipartUploadFileResponse.MediaAssetId,
            startMultipartUploadFileResponse.UploadId
            );

        HttpResponseMessage abortMultipartResponseMessage = await AppHttpClient
            .PostAsJsonAsync("/api/files/multipart/abort", request, cancellationToken);

        return await abortMultipartResponseMessage
            .HandleResponseAsync(cancellationToken);
    }

    private async Task<UnitResult<Errors>> CancelMultipartUpload(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        CancellationToken cancellationToken)
    {
        CancelMultipartUploadFileRequest request = new(
            startMultipartUploadFileResponse.MediaAssetId,
            startMultipartUploadFileResponse.UploadId
        );

        HttpResponseMessage cancelMultipartResponseMessage = await AppHttpClient
            .PostAsJsonAsync("/api/files/multipart/cancel", request, cancellationToken);

        return await cancelMultipartResponseMessage
            .HandleResponseAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<PartETagDto>> UploadChunksAsync(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = fileInfo.OpenRead();

        List<PartETagDto> partETags = [];

        foreach (ChunkUploadUrl chunkUploadUrl in startMultipartUploadFileResponse.ChunkUploadUrls
                     .OrderBy(c => c.PartNumber))
        {
            byte[] chunk = new byte[startMultipartUploadFileResponse.ChunkSize];
            int bytesRead = await stream.ReadAsync(chunk, cancellationToken);
            if (bytesRead == 0)
                break;

            var content = new ByteArrayContent(chunk);

            var responseMinioMessage = await HttpClient.PutAsync(chunkUploadUrl.UploadUrl, content, cancellationToken);

            string? eTag = responseMinioMessage.Headers.ETag?.Tag.Trim();

            partETags.Add(new PartETagDto(chunkUploadUrl.PartNumber, eTag!));
        }

        return partETags;
    }

    private async Task<StartMultipartUploadFileResponse> StartMultipartUpload(
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        var request = new StartMultipartUploadFileRequest(
            fileInfo.Name,
            "video",
            "video/mp4",
            fileInfo.Length,
            "department",
            Guid.NewGuid());

        HttpResponseMessage startMultipartResponseMessage = await AppHttpClient
            .PostAsJsonAsync("/api/files/multipart/start", request, cancellationToken);

        Result<StartMultipartUploadFileResponse, Errors> startMultipartResponse = await startMultipartResponseMessage.HandleResponseAsync<StartMultipartUploadFileResponse>(cancellationToken);

        Assert.True(startMultipartResponse.IsSuccess);
        Assert.NotNull(startMultipartResponse.Value.UploadId);

        await ExecuteInDb(async db =>
        {
            var mediaAsset = await db.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartResponse.Value.MediaAssetId, cancellationToken);

            Assert.Equal(MediaStatus.UPLOADING, mediaAsset?.Status);
            Assert.NotNull(mediaAsset);
        });

        return startMultipartResponse.Value;
    }
}