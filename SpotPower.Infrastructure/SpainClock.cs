namespace SpotPower.Infrastructure;

/// <summary>Single source of truth for Spain-local (Europe/Madrid, CET/CEST) time conversions.</summary>
public static class SpainClock
{
    public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");

    public static DateTimeOffset ToLocal(DateTime utc)
    {
        var utcOffset = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
        return TimeZoneInfo.ConvertTime(utcOffset, TimeZone);
    }

    public static DateOnly Today() => DateOnly.FromDateTime(ToLocal(DateTime.UtcNow).DateTime);
}
