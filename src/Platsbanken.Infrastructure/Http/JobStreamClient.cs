using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Streaming;
using Platsbanken.Infrastructure.Mapping;
using Platsbanken.Infrastructure.Wire;

namespace Platsbanken.Infrastructure.Http;

/// <summary>Adapter for the JobStream API (<see cref="IJobAdStream"/>), reading JSON Lines lazily.</summary>
internal sealed class JobStreamClient(HttpClient http) : IJobAdStream
{
    private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss";
    private static readonly MediaTypeWithQualityHeaderValue JsonLines = new("application/jsonl");

    public IAsyncEnumerable<JobAd> SnapshotAsync(CancellationToken cancellationToken = default)
        => ReadAsync("v2/snapshot", cancellationToken);

    public IAsyncEnumerable<JobAd> StreamAsync(StreamWindow window, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(window);
        return ReadAsync(BuildStreamUri(window), cancellationToken);
    }

    internal static string BuildStreamUri(StreamWindow w)
    {
        var parts = new List<string> { "updated-after=" + Format(w.UpdatedAfter) };
        if (w.UpdatedBefore is { } before)
        {
            parts.Add("updated-before=" + Format(before));
        }

        parts.AddRange(w.OccupationConceptIds.Select(x => "occupation-concept-id=" + Uri.EscapeDataString(x.Value)));
        parts.AddRange(w.LocationConceptIds.Select(x => "location-concept-id=" + Uri.EscapeDataString(x.Value)));
        return "v2/stream?" + string.Join('&', parts);
    }

    // The API takes local Swedish time without an offset.
    private static string Format(DateTimeOffset value)
        => Uri.EscapeDataString(JobAdMapper.ToLocalStockholm(value).ToString(TimestampFormat, CultureInfo.InvariantCulture));

    private async IAsyncEnumerable<JobAd> ReadAsync(
        string uri,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(JsonLines);

        // Headers only: the body is consumed incrementally so memory stays flat for large snapshots.
        using var response = await http
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var body = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(body);

        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var wire = JsonSerializer.Deserialize(line, WireJsonContext.Default.WireJobAd);
            if (JobAdMapper.ToDomain(wire) is { } ad)
            {
                yield return ad;
            }
        }
    }
}
