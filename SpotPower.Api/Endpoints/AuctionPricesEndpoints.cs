using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpotPower.Infrastructure;
using SpotPower.Infrastructure.Persistence;

namespace SpotPower.Api.Endpoints;

public static class AuctionPricesEndpoints
{
    public static void MapAuctionPricesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/auction-prices", GetAuctionPrices)
            .WithName("GetAuctionPrices")
            .Produces<IEnumerable<AuctionPricePoint>>(StatusCodes.Status200OK, "application/json")
            .Produces<string>(StatusCodes.Status200OK, "text/plain")
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAuctionPrices(
        DateOnly? from,
        DateOnly? to,
        HttpRequest request,
        SpotPowerDbContext db,
        CancellationToken ct)
    {
        var today = SpainClock.Today();
        var fromDate = from ?? today;
        var toDate = to ?? today.AddDays(1);

        if (toDate < fromDate)
        {
            return Results.BadRequest("'to' must not be earlier than 'from'.");
        }

        var rows = await db.AuctionPrices
            .Where(p => p.DeliveryDate >= fromDate && p.DeliveryDate <= toDate)
            .OrderBy(p => p.PeriodStartUtc)
            .ToListAsync(ct);

        var points = rows.Select(p => new AuctionPricePoint(
            SpainClock.ToLocal(p.PeriodStartUtc),
            p.PriceEurPerMwhSpain,
            p.PriceEurPerMwhPortugal)).ToList();

        return WantsPlainText(request) ? Results.Text(ToCsv(points), "text/plain") : Results.Json(points);
    }

    // Only two content types are in scope here (JSON default, text/plain CSV), so a full
    // RFC 7231 q-value negotiation isn't needed - just check whether text/plain was requested.
    private static bool WantsPlainText(HttpRequest request) =>
        request.Headers.Accept.Any(v => v is not null && v.Contains("text/plain", StringComparison.OrdinalIgnoreCase));

    private static string ToCsv(IEnumerable<AuctionPricePoint> points)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TimestampCet;PriceEurPerMwhSpain;PriceEurPerMwhPortugal");
        foreach (var p in points)
        {
            sb.AppendLine(string.Join(';',
                p.TimestampCet.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
                p.PriceEurPerMwhSpain.ToString("F2", CultureInfo.InvariantCulture),
                p.PriceEurPerMwhPortugal.ToString("F2", CultureInfo.InvariantCulture)));
        }
        return sb.ToString();
    }
}
