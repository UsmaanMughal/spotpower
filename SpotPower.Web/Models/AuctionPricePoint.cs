namespace SpotPower.Web.Models;

/// <summary>
/// Mirrors the REST API's response shape. Deliberately not shared via a project
/// reference to SpotPower.Core - this project talks to the API over HTTP only.
/// </summary>
public record AuctionPricePoint(DateTimeOffset TimestampCet, decimal PriceEurPerMwhSpain, decimal PriceEurPerMwhPortugal);
