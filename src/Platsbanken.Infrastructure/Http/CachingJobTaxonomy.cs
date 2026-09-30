using System.Collections.Concurrent;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Taxonomy;

namespace Platsbanken.Infrastructure.Http;

/// <summary>Decorator: keeps each concept type in memory for a fixed duration. Taxonomy changes rarely.</summary>
internal sealed class CachingJobTaxonomy(IJobTaxonomy inner, TimeProvider clock, TimeSpan duration) : IJobTaxonomy
{
    private sealed record Entry(Lazy<Task<IReadOnlyList<Concept>>> Load, DateTimeOffset ExpiresAt);

    private readonly ConcurrentDictionary<string, Entry> _cache = new();

    public async Task<IReadOnlyList<Concept>> GetConceptsAsync(ConceptType type, CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();
        var entry = _cache.AddOrUpdate(
            type.Value,
            _ => Create(type, now),
            (_, existing) => existing.ExpiresAt > now ? existing : Create(type, now));

        try
        {
            return await entry.Load.Value.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch when (entry.Load.IsValueCreated && entry.Load.Value.IsFaulted)
        {
            // Never cache a failure: drop the entry so the next call retries.
            _cache.TryRemove(new KeyValuePair<string, Entry>(type.Value, entry));
            throw;
        }
    }

    // The shared load must not be tied to the cancellation token of whichever caller came first.
    private Entry Create(ConceptType type, DateTimeOffset now)
        => new(new Lazy<Task<IReadOnlyList<Concept>>>(() => inner.GetConceptsAsync(type, CancellationToken.None)), now + duration);
}
