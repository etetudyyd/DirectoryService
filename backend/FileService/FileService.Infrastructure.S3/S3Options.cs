namespace DirectoryService;

public record S3Options
{
    public string Endpoint { get; init; } = string.Empty;

    public string AccessKey { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public bool WithSsl { get; init; }

    public int UploadUrlExpirationHours { get; init; }
    public int DownloadUrlExpirationHours { get; init; } = 24;

    public int MaxConcurrentRequests { get; init; } = 20;

    public int RecommendedChunkSizeBytes { get; set; } = 10 * 1024 * 1024;

    public int MaxChunks { get; set; } = 10_000;
    public IReadOnlyList<string> RequiredBuckets { get; init; } = [];
}