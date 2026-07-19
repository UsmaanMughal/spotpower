namespace SpotPower.Api.Endpoints;

/// <summary>One 15-minute settlement period, timestamped in CET/CEST (Europe/Madrid).</summary>
public record AuctionPricePoint(DateTimeOffset TimestampCet, decimal PriceEurPerMwhSpain, decimal PriceEurPerMwhPortugal);
