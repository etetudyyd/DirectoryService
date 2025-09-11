namespace DevQuestions.Domain;

public readonly struct LengthConstants
{
    public static readonly int MAX_LENGTH_DEPARTMENT_NAME = 150;
    public static readonly int MIN_LENGTH_DEPARTMENT_NAME = 3;
    public const int MAX_LENGTH_DEPARTMENT_IDENTIFIER = 150;
    public const int MIN_LENGTH_DEPARTMENT_IDENTIFIER = 3;
    public const int MAX_LENGTH_DEPARTMENT_PATH = 9999;

    public const int MAX_LENGTH_LOCATION_NAME = 120;
    public const int MIN_LENGTH_LOCATION_NAME = 3;

    public static class Address
    {
        public const int MAX_LENGTH_ADDRESS_STREET = 100;
        public const int MAX_LENGTH_ADDRESS_CITY = 100;
        public const int MAX_LENGTH_ADDRESS_HOUSE_NUMBER = 100;
        public const int MAX_LENGTH_ADDRESS_ZIP_CODE = 100;
    }

    public const int MAX_LENGTH_POSITION_NAME = 100;
    public const int MIN_LENGTH_POSITION_NAME = 3;

    public const int MAX_LENGTH_DESCRIPTION = 1000;
}