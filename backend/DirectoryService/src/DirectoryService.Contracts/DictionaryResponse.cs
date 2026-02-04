namespace DirectoryService;

public record DictionaryResponse(IReadOnlyList<DictionaryItemResponse> Items);