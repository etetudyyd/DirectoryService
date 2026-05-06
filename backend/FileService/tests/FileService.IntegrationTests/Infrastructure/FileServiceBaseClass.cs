using Amazon.S3;
using CSharpFunctionalExtensions;
using DirectoryService.Assets;
using DirectoryService.FilesStorage;
using DirectoryService.Types;
using DirectoryService.VOs;
using Microsoft.Extensions.DependencyInjection;
using Shared.SharedKernel;

namespace DirectoryService.Infrastructure;

public class FileServiceBaseClass : IClassFixture<FileServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    private readonly IServiceProvider _serviceProvider;

    protected HttpClient AppHttpClient { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected FileServiceBaseClass(FileServiceTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        _serviceProvider = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _resetDatabase();
        AppHttpClient.Dispose();
        HttpClient.Dispose();
    }

    private async Task<TResult> Execute<TResult, TService>(Func<TService, Task<TResult>> action)
        where TService : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TService handler = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(handler);
    }

    private async Task Execute<TService>(Func<TService, Task> action)
        where TService : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TService handler = scope.ServiceProvider.GetRequiredService<TService>();
        await action(handler);
    }

    protected async Task<T> ExecuteHandler<TCommand, T>(Func<TCommand, Task<T>> action)
        where TCommand : notnull
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TCommand sut = scope.ServiceProvider.GetRequiredService<TCommand>();
        return await action(sut);
    }

    protected async Task<MediaAsset> CreateVideoAssetAsync(
        FileInfo fileInfo,
        CancellationToken cancellationToken) => await CreateMediaAssetAsync(
        fileInfo,
        AssetType.VIDEO,
        "video/mp4",
        MediaStatus.UPLOADED,
        cancellationToken);

    protected async Task<MediaAsset> CreateVideoAssetReadyAsync(
        FileInfo fileInfo,
        CancellationToken cancellationToken) => await CreateMediaAssetAsync(
        fileInfo,
        AssetType.VIDEO,
        "video/mp4",
        MediaStatus.READY,
        cancellationToken);

    protected async Task<MediaAsset> CreatePreviewAssetAsync(
        FileInfo fileInfo,
        CancellationToken cancellationToken) => await CreateMediaAssetAsync(
        fileInfo,
        AssetType.PREVIEW,
        "image/png",
        MediaStatus.UPLOADED,
        cancellationToken);

    private async Task<MediaAsset> CreateMediaAssetAsync(
        FileInfo fileInfo,
        AssetType assetType,
        string contentType,
        MediaStatus status,
        CancellationToken cancellationToken)
    {
        var mediaAsset = await ExecuteInDb(async dbContext =>
        {
            var mediaData = MediaData.Create(
                FileName.Create(fileInfo.Name).Value,
                ContentType.Create(contentType).Value,
                fileInfo.Length,
                1).Value;

            var mediaAsset = MediaAsset.CreateForUpload(
                mediaData,
                assetType,
                MediaOwner.ForDepartment(Guid.NewGuid()).Value).Value;

            dbContext.MediaAssets.Add(mediaAsset);
            await dbContext.SaveChangesAsync(cancellationToken);

            await ExecuteInS3Provider(async s3Provider =>
            {
                await s3Provider.UploadFileAsync(mediaAsset.RawKey, fileInfo.OpenRead(), mediaData, cancellationToken);
            });

            if (status != MediaStatus.UPLOADING)
            {
                switch (status)
                {
                    case MediaStatus.FAILED: mediaAsset.MarkFailed(DateTime.UtcNow); break;

                    case MediaStatus.UPLOADED: mediaAsset.MarkUploaded(DateTime.UtcNow); break;

                    case MediaStatus.DELETED:
                        {
                            mediaAsset.MarkUploaded(DateTime.UtcNow);
                            mediaAsset.MarkDeleted(DateTime.UtcNow);
                            break;
                        }

                    case MediaStatus.READY:
                        {
                            mediaAsset.MarkUploaded(DateTime.UtcNow);
                            mediaAsset.MarkReady(mediaAsset.RawKey, DateTime.UtcNow);
                            break;
                        }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return mediaAsset;
        });

        return mediaAsset;
    }

    protected async Task<T> ExecuteInDb<T>(Func<FileServiceDbContext, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInDb(Func<FileServiceDbContext, Task> action) => await Execute(action);

    protected async Task<T> ExecuteInS3Client<T>(Func<IAmazonS3, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInS3Client(Func<IAmazonS3, Task> action) => await Execute(action);

    protected async Task<T> ExecuteInS3Provider<T>(Func<IS3Provider, Task<T>> action) => await Execute(action);

    protected async Task ExecuteInS3Provider(Func<IS3Provider, Task> action) => await Execute(action);

}