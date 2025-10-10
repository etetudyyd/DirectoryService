namespace DirectoryService.Contracts.Locations.Responses;

public record GetLocationsResponse(
    Guid[] DepartmentsIds,
    string Search,
    bool Active
    //int Page,
    //int PageSize
    );