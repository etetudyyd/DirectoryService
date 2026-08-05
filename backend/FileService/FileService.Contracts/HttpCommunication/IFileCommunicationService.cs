using CSharpFunctionalExtensions;
using DirectoryService.Requests;
using DirectoryService.Responses;
using Shared.SharedKernel;

namespace DirectoryService.HttpCommunication;

public interface IFileCommunicationService
{
    Task<Result<GetMediaAssetsInfoResponse, Errors>> GetMediaAssetsInfoAsync(
        GetMediaAssetsInfoRequest request, CancellationToken cancellationToken);

    Task<Result<CheckMediaAssetExistsResponse, Errors>> CheckMediaAssetExists(
        Guid mediaAssetId, CancellationToken cancellationToken);

}