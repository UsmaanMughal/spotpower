using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpotPower.Infrastructure.Omie;

/// <summary>
/// Downloads OMIE's public "marginalpdbc" day-ahead price file.
/// Verified format (2026-07): https://www.omie.es/en/file-download?parents[0]=marginalpdbc&amp;filename=marginalpdbc_YYYYMMDD.1
/// No authentication required. Returns 404 for dates that haven't been published yet.
/// </summary>
public class OmieClient(HttpClient httpClient, IOptions<OmieOptions> options, ILogger<OmieClient> logger)
{
    private readonly OmieOptions _options = options.Value;

    /// <summary>Returns the raw file content, or null if the file isn't published yet (HTTP 404).</summary>
    public async Task<string?> TryDownloadMarginalPricesAsync(DateOnly deliveryDate, CancellationToken cancellationToken)
    {
        var fileName = $"marginalpdbc_{deliveryDate:yyyyMMdd}.1";
        var url = $"{_options.BaseUrl}?parents%5B0%5D=marginalpdbc&filename={fileName}";

        logger.LogDebug("Requesting OMIE file {FileName} from {Url}", fileName, url);

        using var response = await httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogDebug("OMIE file {FileName} not published yet", fileName);
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
