using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

/// <summary>Port: query currently published job ads.</summary>
public interface IJobAdSearch
{
    Task<JobSearchResult> SearchAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>Returns the ad, or null when it does not exist or has been removed.</summary>
    Task<JobAd?> GetAsync(JobAdId id, CancellationToken cancellationToken = default);
}
