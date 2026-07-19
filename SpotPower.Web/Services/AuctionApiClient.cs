using System.Net.Http.Json;
using System.Text.Json;
using SpotPower.Web.Models;

namespace SpotPower.Web.Services;

/// <summary>The only way this project touches auction data - HTTP calls to the REST API, nothing else.</summary>
public class AuctionApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Omitting from/to lets the API apply its own default range (Spain-local today..tomorrow) -
    /// this project intentionally never computes "today" itself, to avoid duplicating the API's
    /// Europe/Madrid timezone logic.
    /// </summary>
    public async Task<List<AuctionPricePoint>> GetPricesAsync(DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var query = string.Join('&', new[]
        {
            from is { } f ? $"from={f:yyyy-MM-dd}" : null,
            to is { } t ? $"to={t:yyyy-MM-dd}" : null,
        }.Where(p => p is not null));

        var url = query.Length == 0 ? "/api/auction-prices" : $"/api/auction-prices?{query}";
        var result = await httpClient.GetFromJsonAsync<List<AuctionPricePoint>>(url, JsonOptions, ct);
        return result ?? [];
    }
}
