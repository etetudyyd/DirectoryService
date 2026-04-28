using DirectoryService.VOs;

namespace DirectoryService.Models;

public record MediaUrl(StorageKey StorageKey, string PresignedUrl);