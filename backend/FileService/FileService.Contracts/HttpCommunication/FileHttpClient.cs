using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Requests;
using DirectoryService.Responses;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.HttpCommunication;

internal sealed class FileHttpClient : IFileCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileHttpClient> _logger;

    public FileHttpClient(HttpClient httpClient, ILogger<FileHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<GetMediaAssetsInfoResponse, Errors>> GetMediaAssetsInfoAsync(
        GetMediaAssetsInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"/files/batch", request, cancellationToken);

            return await response.HandleResponseAsync<GetMediaAssetsInfoResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred when getting media assets by ids {Ids}",
                string.Join(", ", request.MediaAssetIds));
            return Error.Failure("server.error", "Failed to get media assets info").ToErrors();
        }
    }

    public async Task<Result<CheckMediaAssetExistsResponse, Errors>> CheckMediaAssetExists(
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/files/{mediaAssetId}/exists", cancellationToken);

            return await response.HandleResponseAsync<CheckMediaAssetExistsResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking media asset with id {Id}", mediaAssetId);

            return Error.Failure("server.error", "Failed to check media assets exists").ToErrors();
        }
    }
}
