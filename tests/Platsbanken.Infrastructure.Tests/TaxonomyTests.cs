using Microsoft.Extensions.Time.Testing;
using Platsbanken.Application;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Taxonomy;
using Platsbanken.Infrastructure.Http;

namespace Platsbanken.Infrastructure.Tests;

public class TaxonomyTests
{
    private static readonly Concept Stockholm = new(new ConceptId("s"), "Stockholm");
    private static readonly Concept Solna = new(new ConceptId("o"), "Solna");

    [Fact]
    public async Task Cache_calls_inner_once_within_duration_then_reloads()
    {
        var inner = new CountingTaxonomy();
        var clock = new FakeTimeProvider();
        var cache = new CachingJobTaxonomy(inner, clock, TimeSpan.FromHours(1));

        await cache.GetConceptsAsync(ConceptType.Municipality);
        await cache.GetConceptsAsync(ConceptType.Municipality);
        Assert.Equal(1, inner.Calls);

        await cache.GetConceptsAsync(ConceptType.Region);
        Assert.Equal(2, inner.Calls);

        clock.Advance(TimeSpan.FromHours(2));
        await cache.GetConceptsAsync(ConceptType.Municipality);
        Assert.Equal(3, inner.Calls);
    }

    [Fact]
    public async Task Cache_does_not_keep_failures()
    {
        var inner = new CountingTaxonomy { FailFirst = true };
        var cache = new CachingJobTaxonomy(inner, new FakeTimeProvider(), TimeSpan.FromHours(1));

        await Assert.ThrowsAsync<InvalidOperationException>(() => cache.GetConceptsAsync(ConceptType.Region));
        var result = await cache.GetConceptsAsync(ConceptType.Region);

        Assert.Equal(2, inner.Calls);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Service_finds_by_label_and_searches_substring()
    {
        var service = new TaxonomyService(new CountingTaxonomy());

        Assert.Equal(Stockholm, await service.FindByLabelAsync(ConceptType.Municipality, "stockholm"));
        Assert.Null(await service.FindByLabelAsync(ConceptType.Municipality, "Malmo"));
        Assert.Equal([Solna], await service.SearchAsync(ConceptType.Municipality, "sol"));
    }

    private sealed class CountingTaxonomy : IJobTaxonomy
    {
        public int Calls { get; private set; }
        public bool FailFirst { get; init; }

        public Task<IReadOnlyList<Concept>> GetConceptsAsync(ConceptType type, CancellationToken cancellationToken = default)
        {
            Calls++;
            if (FailFirst && Calls == 1)
            {
                return Task.FromException<IReadOnlyList<Concept>>(new InvalidOperationException("boom"));
            }

            return Task.FromResult<IReadOnlyList<Concept>>([Stockholm, Solna]);
        }
    }
}
