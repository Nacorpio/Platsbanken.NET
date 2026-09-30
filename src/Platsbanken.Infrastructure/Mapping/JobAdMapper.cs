using System.Globalization;
using Platsbanken.Domain.Ads;
using Platsbanken.Infrastructure.Wire;

namespace Platsbanken.Infrastructure.Mapping;

/// <summary>Anti-corruption layer: wire DTOs to the domain model.</summary>
internal static class JobAdMapper
{
    private static readonly TimeZoneInfo Stockholm = ResolveStockholm();

    public static JobAd? ToDomain(WireJobAd? wire)
    {
        if (wire is null || string.IsNullOrWhiteSpace(wire.Id))
        {
            return null;
        }

        return new JobAd
        {
            Id = new JobAdId(wire.Id),
            Headline = wire.Headline,
            WebpageUrl = ToUri(wire.WebpageUrl),
            LogoUrl = ToUri(wire.LogoUrl),
            Description = wire.Description?.Text,
            NumberOfVacancies = wire.NumberOfVacancies,
            PublicationDate = ToTimestamp(wire.PublicationDate),
            LastPublicationDate = ToTimestamp(wire.LastPublicationDate),
            ApplicationDeadline = ToTimestamp(wire.ApplicationDeadline),
            Employer = wire.Employer is { } e
                ? new Employer(e.Name, e.OrganizationNumber, e.Workplace, e.Url, e.Email, e.PhoneNumber)
                : null,
            Occupation = ToConcept(wire.Occupation),
            OccupationGroup = ToConcept(wire.OccupationGroup),
            OccupationField = ToConcept(wire.OccupationField),
            EmploymentType = ToConcept(wire.EmploymentType),
            Workplace = ToWorkplace(wire.WorkplaceAddress),
            ScopeOfWorkMin = wire.ScopeOfWork?.Min,
            ScopeOfWorkMax = wire.ScopeOfWork?.Max,
            DrivingLicenseRequired = wire.DrivingLicenseRequired ?? false,
            ExperienceRequired = wire.ExperienceRequired ?? false,
            IsRemoved = wire.Removed ?? false,
        };
    }

    private static Concept? ToConcept(WireConcept? wire)
        => wire is { ConceptId: { Length: > 0 } id } ? new Concept(new ConceptId(id), wire.Label ?? string.Empty) : null;

    private static Concept? ToConcept(string? id, string? label)
        => string.IsNullOrWhiteSpace(id) ? null : new Concept(new ConceptId(id), label ?? string.Empty);

    private static Workplace? ToWorkplace(WireWorkplaceAddress? wire)
    {
        if (wire is null)
        {
            return null;
        }

        // The API delivers coordinates as [longitude, latitude], and sometimes as [null, null].
        var point = wire.Coordinates is { Length: 2 } c && c[0] is { } lon && c[1] is { } lat ? new GeoPoint(lon, lat) : null;
        return new Workplace(
            ToConcept(wire.MunicipalityConceptId, wire.Municipality),
            ToConcept(wire.RegionConceptId, wire.Region),
            ToConcept(wire.CountryConceptId, wire.Country),
            wire.City, wire.StreetAddress, wire.Postcode, point);
    }

    private static Uri? ToUri(string? value)
        => Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;

    /// <summary>The API emits local Swedish time without an offset (YYYY-MM-DDTHH:MM:SS).</summary>
    internal static DateTimeOffset? ToTimestamp(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
        {
            return null;
        }

        local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        return new DateTimeOffset(local, Stockholm.GetUtcOffset(local));
    }

    /// <summary>Converts an instant to local Swedish wall-clock time, as the API expects in queries.</summary>
    internal static DateTime ToLocalStockholm(DateTimeOffset value)
        => TimeZoneInfo.ConvertTime(value, Stockholm).DateTime;

    private static TimeZoneInfo ResolveStockholm()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
