namespace Platsbanken.Domain.Ads;

/// <summary>A WGS84 coordinate.</summary>
/// <param name="Longitude">Longitude in degrees.</param>
/// <param name="Latitude">Latitude in degrees.</param>
public sealed record GeoPoint(double Longitude, double Latitude);

/// <summary>Where the work is performed.</summary>
/// <param name="Municipality">Municipality concept (kommun).</param>
/// <param name="Region">Region concept (län).</param>
/// <param name="Country">Country concept.</param>
/// <param name="City">City name as written in the ad.</param>
/// <param name="StreetAddress">Street address, when published.</param>
/// <param name="Postcode">Postcode, when published.</param>
/// <param name="Coordinates">Geographic position, when the API provides one.</param>
public sealed record Workplace(
    Concept? Municipality,
    Concept? Region,
    Concept? Country,
    string? City,
    string? StreetAddress,
    string? Postcode,
    GeoPoint? Coordinates);
