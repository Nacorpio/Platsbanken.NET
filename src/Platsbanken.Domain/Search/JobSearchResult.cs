using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

/// <summary>One page of search results.</summary>
/// <param name="Total">Total number of matching ads, across all pages.</param>
/// <param name="Hits">The ads on this page.</param>
public sealed record JobSearchResult(int Total, IReadOnlyList<JobAd> Hits);
