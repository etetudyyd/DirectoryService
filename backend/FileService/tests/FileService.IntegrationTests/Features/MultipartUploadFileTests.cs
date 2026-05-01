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
    public async Task MultipartUpload_FullCycle_PersistsMediaFile()
    {
        // arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-video.mp4"));

        // act
        StartMultipartUploadFileResponse startMultipartUploadFileResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        IReadOnlyList<PartETagDto> partETagDtoS = await UploadChunks(startMultipartUploadFileResponse, fileInfo, cancellationToken);

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

        UnitResult<Errors> completeMultipartResponse = await completeMultipartResponseMessage
            .HandleResponseAsync(cancellationToken);

        return completeMultipartResponse;
    }

    private async Task<IReadOnlyList<PartETagDto>> UploadChunks(
        StartMultipartUploadFileResponse startMultipartUploadFileResponse,
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = fileInfo.OpenRead();

        List<PartETagDto> partETags = [];

        foreach (ChunkUploadUrl chunkUploadUrl in startMultipartUploadFileResponse.ChunkUploadUrls.OrderBy(c => c.PartNumber))
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