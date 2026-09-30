using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

public sealed record JobSearchResult(int Total, IReadOnlyList<JobAd> Hits);
