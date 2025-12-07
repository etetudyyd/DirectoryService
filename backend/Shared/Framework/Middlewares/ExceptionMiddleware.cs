using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace Framework.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var apiError = new Error("server.internal", ex.Message, ErrorType.FAILURE);
            var envelope = Envelope.Error(new Errors([apiError]));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError(ex.Message);
            await context.Response.WriteAsJsonAsync(envelope);
        }
    }
}

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this WebApplication app) =>
            app.UseMiddleware<ExceptionMiddleware>();
    }
