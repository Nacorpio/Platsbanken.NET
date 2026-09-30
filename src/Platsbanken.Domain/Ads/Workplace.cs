namespace Platsbanken.Domain.Ads;

public sealed record GeoPoint(double Longitude, double Latitude);

public sealed record Workplace(
    Concept? Municipality,
    Concept? Region,
    Concept? Country,
    string? City,
    string? StreetAddress,
    string? Postcode,
    GeoPoint? Coordinates);
