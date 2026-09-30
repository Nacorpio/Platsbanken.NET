using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

/// <summary>Port: query currently published job ads.</summary>
public interface IJobAdSearch
{
    /// <summary>Searches published ads.</summary>
    /// <param name="criteria">Filters and paging. Validated before the request is sent.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    Task<JobSearchResult> SearchAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>Returns the ad, or null when it does not exist or has been removed.</summary>
    /// <param name="id">The ad id.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    Task<JobAd?> GetAsync(JobAdId id, CancellationToken cancellationToken = default);
}
