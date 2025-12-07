namespace Core.Options;

public sealed class TimeOptions
{
    public DateTime InputDate { get; private set; }

    public TimeOptions(DateTime inputDate)
    {
        InputDate = inputDate;
    }

    public static TimeOptions FromDaysAgo(int days)
    {
        return new TimeOptions(DateTime.Now.AddDays(-days));
    }

    public static TimeOptions FromHoursAgo(int hours)
    {
        return new TimeOptions(DateTime.Now.AddHours(-hours));
    }

    public static TimeOptions FromMonthsAgo(int months)
    {
        return new TimeOptions(DateTime.Now.AddMonths(-months));
    }

    public static TimeOptions FromMinutesAgo(int minutes)
    {
        return new TimeOptions(DateTime.Now.AddMinutes(-minutes));
    }

}