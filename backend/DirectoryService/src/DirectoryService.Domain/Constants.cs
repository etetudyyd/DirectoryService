namespace DirectoryService;

public readonly struct Constants
{
    // Routes
    public const string DEPARTMENT_TABLE_ROUTE = "\"DirectoryService\".\"departments\"";
    public const string LOCATION_TABLE_ROUTE = "\"DirectoryService\".\"locations\"";
    public const string POSITION_TABLE_ROUTE = "\"DirectoryService\".\"positions\"";
    public const string DEPARTMENT_POSITIONS_TABLE_ROUTE = "\"DirectoryService\".\"department_positions\"";
    public const string DEPARTMENT_LOCATIONS_TABLE_ROUTE = "\"DirectoryService\".\"department_locations\"";

    // General
    public const string SOFT_DELETED_LABEL = "deleted-";

    // Cache Prefixes
    public const string DEPARTMENT_CACHE_PREFIX = "department";
    public const string LOCATION_CACHE_PREFIX = "location";
    public const string POSITION_CACHE_PREFIX = "position";

    // ttl
    public const int TTL_CACHE = 5;

    // lenght constants
    public static readonly int MAX_LENGTH_DEPARTMENT_NAME = 150;
    public static readonly int MIN_LENGTH_DEPARTMENT_NAME = 3;
    public const int MAX_LENGTH_DEPARTMENT_IDENTIFIER = 150;
    public const int MIN_LENGTH_DEPARTMENT_IDENTIFIER = 3;
    public const int MAX_LENGTH_DEPARTMENT_PATH = 9999;

    public const int MAX_LENGTH_LOCATION_NAME = 120;
    public const int MIN_LENGTH_LOCATION_NAME = 3;

    public static class Address
    {
        public const int MAX_LENGTH_ADDRESS_POSTAL_CODE = 100;
        public const int MAX_LENGTH_ADDRESS_REGION = 100;
        public const int MAX_LENGTH_ADDRESS_CITY = 100;
        public const int MAX_LENGTH_ADDRESS_STREET = 100;
        public const int MAX_LENGTH_ADDRESS_HOUSE = 100;
        public const int MAX_LENGTH_ADDRESS_APARTAMENT = 100;

    }

    public const int LENGTH_ADDRESS_POSTAL_CODE = 6;
    public const int MAX_LENGTH_POSITION_NAME = 100;
    public const int MIN_LENGTH_POSITION_NAME = 3;

    public const int MAX_LENGTH_DESCRIPTION = 1000;
}