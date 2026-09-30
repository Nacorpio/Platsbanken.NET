using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Search;
using Platsbanken.Infrastructure.Mapping;
using Platsbanken.Infrastructure.Wire;

namespace Platsbanken.Infrastructure.Http;

/// <summary>Adapter for the JobSearch API (<see cref="IJobAdSearch"/>).</summary>
internal sealed class JobSearchClient(HttpClient http) : IJobAdSearch
{
    private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss";

    public async Task<JobSearchResult> SearchAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        criteria.Validate();

        using var response = await http.GetAsync(BuildSearchUri(criteria), cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var wire = await response.Content
            .ReadFromJsonAsync(WireJsonContext.Default.WireSearchResults, cancellationToken)
            .ConfigureAwait(false);

        var hits = (wire?.Hits ?? [])
            .Select(JobAdMapper.ToDomain)
            .OfType<JobAd>()
            .ToList();
        return new JobSearchResult(wire?.Total?.Value ?? hits.Count, hits);
    }

    public async Task<JobAd?> GetAsync(JobAdId id, CancellationToken cancellationToken = default)
    {
        using var response = await http
            .GetAsync($"ad/{Uri.EscapeDataString(id.Value)}", cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var wire = await response.Content
            .ReadFromJsonAsync(WireJsonContext.Default.WireJobAd, cancellationToken)
            .ConfigureAwait(false);
        var ad = JobAdMapper.ToDomain(wire);
        return ad is { IsRemoved: true } ? null : ad;
    }

    internal static string BuildSearchUri(JobSearchCriteria c)
    {
        var q = new QueryBuilder();
        q.Add("q", c.Query);
        q.AddMany("occupation-name", c.Occupations.Select(x => x.Value));
        q.AddMany("occupation-group", c.OccupationGroups.Select(x => x.Value));
        q.AddMany("occupation-field", c.OccupationFields.Select(x => x.Value));
        q.AddMany("municipality", c.Municipalities.Select(x => x.Value));
        q.AddMany("region", c.Regions.Select(x => x.Value));
        q.Add("employer", c.Employer);
        if (c.PublishedAfter is { } after)
        {
            q.Add("published-after", JobAdMapper.ToLocalStockholm(after).ToString(TimestampFormat, CultureInfo.InvariantCulture));
        }

        if (c.Remote is { } remote)
        {
            q.Add("remote", remote ? "true" : "false");
        }

        q.Add("offset", c.Offset.ToString(CultureInfo.InvariantCulture));
        q.Add("limit", c.Limit.ToString(CultureInfo.InvariantCulture));
        return "search?" + q;
    }

    private sealed class QueryBuilder
    {
        private readonly StringBuilder _sb = new();

        public void Add(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (_sb.Length > 0)
            {
                _sb.Append('&');
            }

            _sb.Append(key).Append('=').Append(Uri.EscapeDataString(value));
        }

        public void AddMany(string key, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                Add(key, value);
            }
        }

        public override string ToString() => _sb.ToString();
    }
}
