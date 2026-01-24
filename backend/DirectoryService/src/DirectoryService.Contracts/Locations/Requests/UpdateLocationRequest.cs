namespace DirectoryService.Locations.Requests;

public record UpdateLocationRequest(
    string Name,
    AddressDto Address,
    string Timezone,
    Guid[] DepartmentsIds);