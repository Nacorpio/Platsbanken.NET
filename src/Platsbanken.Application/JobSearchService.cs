using System.Runtime.CompilerServices;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Search;

namespace Platsbanken.Application;

/// <summary>Use cases over <see cref="IJobAdSearch"/>, including paging past the page-size limit.</summary>
public sealed class JobSearchService(IJobAdSearch search)
{
    public Task<JobSearchResult> SearchAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        criteria.Validate();
        return search.SearchAsync(criteria, cancellationToken);
    }

    public Task<JobAd?> GetAsync(JobAdId id, CancellationToken cancellationToken = default)
        => search.GetAsync(id, cancellationToken);

    /// <summary>
    /// Lazily enumerates every hit for the criteria, page by page. The API caps the offset at
    /// <see cref="JobSearchCriteria.MaxOffset"/>; narrow the criteria or use JobStream beyond that.
    /// </summary>
    public async IAsyncEnumerable<JobAd> SearchAllAsync(
        JobSearchCriteria criteria,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        var page = criteria with { Limit = JobSearchCriteria.MaxLimit };

        while (page.Offset <= JobSearchCriteria.MaxOffset)
        {
            var result = await search.SearchAsync(page, cancellationToken).ConfigureAwait(false);
            foreach (var ad in result.Hits)
            {
                yield return ad;
            }

            var next = page.Offset + JobSearchCriteria.MaxLimit;
            if (result.Hits.Count < JobSearchCriteria.MaxLimit || next >= result.Total)
            {
                yield break;
            }

            page = page with { Offset = next };
        }
    }
}
