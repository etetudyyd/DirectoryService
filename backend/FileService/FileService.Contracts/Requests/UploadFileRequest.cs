using Microsoft.AspNetCore.Http;

namespace DirectoryService.Requests;

public record UploadFileRequest(IFormFile FormFile, string AssetType, string Context, Guid ContextId);