namespace Platsbanken.Domain.Ads;

/// <summary>
/// A job ad as published in Platsbanken. Immutable published-language model:
/// wire-format quirks of the JobTech APIs stay behind the anti-corruption layer.
/// </summary>
public sealed record JobAd
{
    public required JobAdId Id { get; init; }
    public string? Headline { get; init; }
    public Uri? WebpageUrl { get; init; }
    public Uri? LogoUrl { get; init; }
    public string? Description { get; init; }
    public int? NumberOfVacancies { get; init; }
    public DateTimeOffset? PublicationDate { get; init; }
    public DateTimeOffset? LastPublicationDate { get; init; }
    public DateTimeOffset? ApplicationDeadline { get; init; }
    public Employer? Employer { get; init; }
    public Concept? Occupation { get; init; }
    public Concept? OccupationGroup { get; init; }
    public Concept? OccupationField { get; init; }
    public Concept? EmploymentType { get; init; }
    public Workplace? Workplace { get; init; }
    public int? ScopeOfWorkMin { get; init; }
    public int? ScopeOfWorkMax { get; init; }
    public bool DrivingLicenseRequired { get; init; }
    public bool ExperienceRequired { get; init; }

    /// <summary>True when the ad has been unpublished (only seen through the stream).</summary>
    public bool IsRemoved { get; init; }
}
