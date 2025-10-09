namespace DirectoryService.Contracts.Locations;

public record GetLocationDto(Guid Id, string Name, string Address, string Timezone);