using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.HttpCommunication;

public static class HttpResponseMessageExtensions
{
    public static async Task<Result<TResponse, Errors>> HandleResponseAsync<TResponse>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            Envelope<TResponse>? envelope = await response.Content
                .ReadFromJsonAsync<Envelope<TResponse>>(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return envelope?.Errors ?? Error.Failure("test.error", "Unknown error");

            if(envelope is null)
                return Error.Failure("test.error", "Unknown error").ToErrors();

            if (envelope.Errors is not null)
                return envelope.Errors;

            if (envelope.Result is null)
                return Error.Failure("test.error", "Unknown error").ToErrors();

            return envelope.Result;
        }
        catch
        {
            return Error.Failure("test.error", "Error while processing response")
                .ToErrors();
        }
    }

    public static async Task<UnitResult<Errors>> HandleResponseAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Envelope? envelope = await response.Content
                .ReadFromJsonAsync<Envelope>(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return envelope?.ErrorList ?? Error.Failure("test.error", "Unknown error");

            if(envelope is null)
                return Error.Failure("test.error", "Unknown error").ToErrors();

            if (envelope.ErrorList is not null)
                return envelope.ErrorList;

            return UnitResult.Success<Errors>();
        }
        catch
        {
            return Error.Failure("test.error", "Error while processing response")
                .ToErrors();
        }
    }


}