using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SpotPower.Infrastructure.Omie;
using SpotPower.Infrastructure.Persistence;

namespace SpotPower.Infrastructure.Jobs;

/// <summary>
/// Periodically pulls OMIE day-ahead auction results. Each run checks a rolling window of
/// [today - BackfillDays, tomorrow]: tomorrow because that's when the next day-ahead result
/// gets published, and the backfill window because this covers missed runs and restarts
/// without needing any separate "last successful import" bookkeeping.
/// </summary>
[DisallowConcurrentExecution]
public class AuctionImportJob(
    OmieClient omieClient,
    SpotPowerDbContext db,
    IOptions<OmieOptions> options,
    ILogger<AuctionImportJob> logger) : IJob
{
    private readonly OmieOptions _options = options.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        var today = SpainClock.Today();

        for (var date = today.AddDays(-_options.BackfillDays); date <= today.AddDays(1); date = date.AddDays(1))
        {
            await ImportDayIfMissingAsync(date, ct);
        }
    }

    private async Task ImportDayIfMissingAsync(DateOnly date, CancellationToken ct)
    {
        // A day's result is published once and never changes, so if we already have any rows
        // for it, the previous run's SaveChangesAsync committed the whole day - safe to skip.
        var alreadyImported = await db.AuctionPrices.AnyAsync(p => p.DeliveryDate == date, ct);
        if (alreadyImported)
        {
            return;
        }

        string? raw;
        try
        {
            raw = await omieClient.TryDownloadMarginalPricesAsync(date, ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to download OMIE auction results for {Date}", date);
            return;
        }

        if (raw is null)
        {
            logger.LogInformation("OMIE auction results for {Date} are not published yet", date);
            return;
        }

        var prices = OmieMarginalPriceParser.Parse(raw, date);
        var importedAt = DateTime.UtcNow;
        foreach (var price in prices)
        {
            price.ImportedAtUtc = importedAt;
        }

        db.AuctionPrices.AddRange(prices);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Imported {Count} auction periods for {Date}", prices.Count, date);
    }
}
