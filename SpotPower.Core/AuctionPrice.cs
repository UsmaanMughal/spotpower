namespace SpotPower.Core;

/// <summary>
/// One settlement period of the Spanish day-ahead (OMIE) auction result.
/// OMIE has published in 15-minute resolution since October 2025, so a normal day has
/// 96 periods; DST transition days have 92 (spring-forward) or 100 (fall-back).
/// </summary>
public class AuctionPrice
{
    public int Id { get; set; }

    /// <summary>Delivery date in Spain-local (Europe/Madrid) calendar time.</summary>
    public DateOnly DeliveryDate { get; set; }

    /// <summary>1-based quarter-hour period index within the delivery date, as published by OMIE.</summary>
    public int Period { get; set; }

    /// <summary>Start instant of this period, stored as UTC. See OmieMarginalPriceParser for how this is derived.</summary>
    public DateTime PeriodStartUtc { get; set; }

    public decimal PriceEurPerMwhSpain { get; set; }

    public decimal PriceEurPerMwhPortugal { get; set; }

    public DateTime ImportedAtUtc { get; set; }
}
