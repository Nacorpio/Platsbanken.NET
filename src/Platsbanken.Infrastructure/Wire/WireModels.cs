namespace Platsbanken.Infrastructure.Wire;

// Wire-format DTOs mirroring the JobTech OpenAPI specs. Internal on purpose: the mapper is the
// anti-corruption layer and nothing outside Infrastructure may depend on these shapes.

internal sealed record WireConcept(string? ConceptId, string? Label);

internal sealed record WireEmployer(
    string? PhoneNumber, string? Email, string? Url, string? OrganizationNumber, string? Name, string? Workplace);

internal sealed record WireDescription(string? Text);

internal sealed record WireScopeOfWork(int? Min, int? Max);

internal sealed record WireWorkplaceAddress(
    string? Municipality, string? MunicipalityConceptId,
    string? Region, string? RegionConceptId,
    string? Country, string? CountryConceptId,
    string? StreetAddress, string? Postcode, string? City,
    double?[]? Coordinates);

internal sealed record WireJobAd
{
    public string? Id { get; init; }
    public string? WebpageUrl { get; init; }
    public string? LogoUrl { get; init; }
    public string? Headline { get; init; }
    public string? ApplicationDeadline { get; init; }
    public int? NumberOfVacancies { get; init; }
    public WireDescription? Description { get; init; }
    public WireConcept? EmploymentType { get; init; }
    public WireScopeOfWork? ScopeOfWork { get; init; }
    public WireEmployer? Employer { get; init; }
    public bool? ExperienceRequired { get; init; }
    public bool? DrivingLicenseRequired { get; init; }
    public WireConcept? Occupation { get; init; }
    public WireConcept? OccupationGroup { get; init; }
    public WireConcept? OccupationField { get; init; }
    public WireWorkplaceAddress? WorkplaceAddress { get; init; }
    public string? PublicationDate { get; init; }
    public string? LastPublicationDate { get; init; }
    public bool? Removed { get; init; }
}

internal sealed record WireNumberOfHits(int Value);

internal sealed record WireSearchResults(WireNumberOfHits? Total, List<WireJobAd>? Hits);
