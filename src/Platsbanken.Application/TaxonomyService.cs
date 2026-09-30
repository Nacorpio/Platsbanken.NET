using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Taxonomy;

namespace Platsbanken.Application;

/// <summary>Use cases over <see cref="IJobTaxonomy"/>.</summary>
public sealed class TaxonomyService(IJobTaxonomy taxonomy)
{
    public Task<IReadOnlyList<Concept>> GetAsync(ConceptType type, CancellationToken cancellationToken = default)
        => taxonomy.GetConceptsAsync(type, cancellationToken);

    /// <summary>Case-insensitive exact label match, or null when nothing matches.</summary>
    public async Task<Concept?> FindByLabelAsync(ConceptType type, string label, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        var all = await taxonomy.GetConceptsAsync(type, cancellationToken).ConfigureAwait(false);
        return all.FirstOrDefault(c => string.Equals(c.Label, label, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Case-insensitive substring search over labels.</summary>
    public async Task<IReadOnlyList<Concept>> SearchAsync(ConceptType type, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var all = await taxonomy.GetConceptsAsync(type, cancellationToken).ConfigureAwait(false);
        return all.Where(c => c.Label.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
