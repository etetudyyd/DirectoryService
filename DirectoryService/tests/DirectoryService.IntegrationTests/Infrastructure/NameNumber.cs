namespace DirectoryService.IntegrationTests.Infrastructure;

public static class NameNumber
{
    private static int _department;
    private static int _location;
    private static int _position;

    public static int GetDepartment() =>
        _department++;

    public static int GetLocation() =>
        _location++;

    public static void Reset()
    {
        _department = 0;
        _location = 0;
        _position = 0;
    }
}