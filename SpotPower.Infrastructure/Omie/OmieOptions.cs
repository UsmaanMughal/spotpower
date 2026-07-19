namespace SpotPower.Infrastructure.Omie;

public class OmieOptions
{
    public const string SectionName = "Omie";

    /// <summary>Base URL for OMIE's public file-download endpoint (no auth required).</summary>
    public string BaseUrl { get; set; } = "https://www.omie.es/en/file-download";

    /// <summary>Cron expression for the recurring import job (Quartz format, seconds-first).</summary>
    public string ImportCronSchedule { get; set; } = "0 0/30 * * * ?";

    /// <summary>How many past days to check/backfill on every run, in case earlier days were missed.</summary>
    public int BackfillDays { get; set; } = 5;
}
