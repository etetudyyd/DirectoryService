namespace DirectoryService.Locations.Requests;

public record CreateLocationRequest(
    string Name,
    AddressDto Address,
    string Timezone,
    Guid[] DepartmentsIds);