using Core.Abstractions;
using CSharpFunctionalExtensions;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.SharedKernel;

namespace DirectoryService.Features.Commands.StartMultipartUploadFile;

public class StartMultipartUploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart-upload", async Task<EndpointResult<string>>(
            [FromBody] StartMultipartUploadFileRequest request,
            [FromServices] StartMultipartUploadFileHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new StartMultipartUploadFileCommand();
            return await handler.Handle(command, cancellationToken);
        }).DisableAntiforgery();
    }
}

public class StartMultipartUploadFileHandler : ICommandHandler<string, StartMultipartUploadFileCommand>
{
    public Task<Result<string, Errors>> Handle(StartMultipartUploadFileCommand command, CancellationToken cancellationToken) => throw new NotImplementedException();
}