namespace DirectoryService.HttpCommunication;

public record FileServiceOptions
{
    public string Url { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 5;
}