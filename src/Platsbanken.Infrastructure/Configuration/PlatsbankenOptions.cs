using System.ComponentModel.DataAnnotations;

namespace Platsbanken.Infrastructure.Configuration;

/// <summary>Settings for the JobTech API clients. Defaults target the public production endpoints.</summary>
public sealed class PlatsbankenOptions
{
    /// <summary>Suggested configuration section name.</summary>
    public const string SectionName = "Platsbanken";

    /// <summary>Base address of the JobSearch API.</summary>
    [Required]
    public Uri SearchBaseAddress { get; set; } = new("https://jobsearch.api.jobtechdev.se/");

    /// <summary>Base address of the JobStream API.</summary>
    [Required]
    public Uri StreamBaseAddress { get; set; } = new("https://jobstream.api.jobtechdev.se/");

    /// <summary>Base address of the Taxonomy API.</summary>
    [Required]
    public Uri TaxonomyBaseAddress { get; set; } = new("https://taxonomy.api.jobtechdev.se/");

    /// <summary>How long a taxonomy concept type stays cached in memory.</summary>
    public TimeSpan TaxonomyCacheDuration { get; set; } = TimeSpan.FromHours(12);

    /// <summary>Sent as User-Agent. Identify your application so JobTech can contact you if needed.</summary>
    [Required]
    public string UserAgent { get; set; } = "Platsbanken.NET";

    /// <summary>Timeout for search requests, per attempt.</summary>
    public TimeSpan SearchTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Timeout until response headers arrive for stream and snapshot requests.</summary>
    public TimeSpan StreamTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
