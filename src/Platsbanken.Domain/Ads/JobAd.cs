namespace Platsbanken.Domain.Ads;

/// <summary>
/// A job ad as published in Platsbanken. Immutable published-language model:
/// wire-format quirks of the JobTech APIs stay behind the anti-corruption layer.
/// </summary>
public sealed record JobAd
{
    /// <summary>Ad identifier.</summary>
    public required JobAdId Id { get; init; }

    /// <summary>Ad headline.</summary>
    public string? Headline { get; init; }

    /// <summary>Public web page of the ad on arbetsformedlingen.se.</summary>
    public Uri? WebpageUrl { get; init; }

    /// <summary>Employer logo, when available.</summary>
    public Uri? LogoUrl { get; init; }

    /// <summary>Plain-text ad description.</summary>
    public string? Description { get; init; }

    /// <summary>Number of open positions.</summary>
    public int? NumberOfVacancies { get; init; }

    /// <summary>When the ad was first published.</summary>
    public DateTimeOffset? PublicationDate { get; init; }

    /// <summary>Last day the ad is published.</summary>
    public DateTimeOffset? LastPublicationDate { get; init; }

    /// <summary>Last day to apply.</summary>
    public DateTimeOffset? ApplicationDeadline { get; init; }

    /// <summary>The employer.</summary>
    public Employer? Employer { get; init; }

    /// <summary>Specific occupation (occupation-name).</summary>
    public Concept? Occupation { get; init; }

    /// <summary>Occupation group.</summary>
    public Concept? OccupationGroup { get; init; }

    /// <summary>Occupation field.</summary>
    public Concept? OccupationField { get; init; }

    /// <summary>Type of employment, for example permanent or temporary.</summary>
    public Concept? EmploymentType { get; init; }

    /// <summary>Where the work is performed.</summary>
    public Workplace? Workplace { get; init; }

    /// <summary>Minimum scope of work in percent of full time.</summary>
    public int? ScopeOfWorkMin { get; init; }

    /// <summary>Maximum scope of work in percent of full time.</summary>
    public int? ScopeOfWorkMax { get; init; }

    /// <summary>True when a driving licence is required.</summary>
    public bool DrivingLicenseRequired { get; init; }

    /// <summary>True when prior experience is required.</summary>
    public bool ExperienceRequired { get; init; }

    /// <summary>True when the ad has been unpublished (only seen through the stream).</summary>
    public bool IsRemoved { get; init; }
}
