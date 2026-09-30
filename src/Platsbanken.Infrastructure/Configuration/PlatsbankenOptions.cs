using System.ComponentModel.DataAnnotations;

namespace Platsbanken.Infrastructure.Configuration;

public sealed class PlatsbankenOptions
{
    public const string SectionName = "Platsbanken";

    [Required]
    public Uri SearchBaseAddress { get; set; } = new("https://jobsearch.api.jobtechdev.se/");

    [Required]
    public Uri StreamBaseAddress { get; set; } = new("https://jobstream.api.jobtechdev.se/");

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
