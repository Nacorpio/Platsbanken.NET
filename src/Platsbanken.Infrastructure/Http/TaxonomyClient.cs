using System.Net.Http.Json;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Taxonomy;
using Platsbanken.Infrastructure.Wire;

namespace Platsbanken.Infrastructure.Http;

/// <summary>Uncached adapter for the JobTech Taxonomy API.</summary>
internal sealed class TaxonomyClient(HttpClient http) : IJobTaxonomy
{
    public async Task<IReadOnlyList<Concept>> GetConceptsAsync(ConceptType type, CancellationToken cancellationToken = default)
    {
        var uri = $"v1/taxonomy/main/concepts?type={Uri.EscapeDataString(type.Value)}";
        var wire = await http
            .GetFromJsonAsync(uri, WireJsonContext.Default.ListWireTaxonomyConcept, cancellationToken)
            .ConfigureAwait(false);

        return (wire ?? [])
            .Where(c => !string.IsNullOrWhiteSpace(c.Id))
            .Select(c => new Concept(new ConceptId(c.Id!), c.PreferredLabel ?? string.Empty))
            .ToList();
    }
}
