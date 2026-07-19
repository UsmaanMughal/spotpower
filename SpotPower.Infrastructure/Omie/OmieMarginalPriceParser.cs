using System.Globalization;
using SpotPower.Core;

namespace SpotPower.Infrastructure.Omie;

/// <summary>
/// Parses OMIE's "marginalpdbc" file. Verified layout (2026-07):
///   line 1: "MARGINALPDBC;" header
///   lines 2..N-1: "yyyy;MM;dd;period;priceSpain;pricePortugal;"
///   last line: "*" terminator
/// Period is a 1-based quarter-hour index (96 on a normal day; empirically 92 on the
/// 2026-03-29 spring-forward day, so periods are contiguous 15-minute blocks of real
/// elapsed time, not fixed wall-clock slots). Spain/Portugal column order is inferred
/// from OMIE's own page copy ("...Spanish and Portuguese electricity systems..."),
/// not from an explicit column spec — see README assumptions.
/// </summary>
public static class OmieMarginalPriceParser
{
    public static readonly TimeZoneInfo SpainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");

    public static List<AuctionPrice> Parse(string rawFileContent, DateOnly deliveryDate)
    {
        var localMidnight = DateTime.SpecifyKind(deliveryDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localMidnight, SpainTimeZone);

        var results = new List<AuctionPrice>();

        foreach (var line in rawFileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (line.StartsWith("MARGINALPDBC", StringComparison.OrdinalIgnoreCase) || line.StartsWith('*'))
            {
                continue;
            }

            var fields = line.Split(';', StringSplitOptions.TrimEntries);
            if (fields.Length < 6)
            {
                continue;
            }

            var period = int.Parse(fields[3], CultureInfo.InvariantCulture);

            results.Add(new AuctionPrice
            {
                DeliveryDate = deliveryDate,
                Period = period,
                // Periods are contiguous 15-minute blocks of real elapsed time (see class remarks),
                // so this stays correct across DST transitions - it is NOT wall-clock addition.
                PeriodStartUtc = dayStartUtc.AddMinutes((period - 1) * 15),
                PriceEurPerMwhSpain = decimal.Parse(fields[4], CultureInfo.InvariantCulture),
                PriceEurPerMwhPortugal = decimal.Parse(fields[5], CultureInfo.InvariantCulture),
            });
        }

        return results;
    }
}
