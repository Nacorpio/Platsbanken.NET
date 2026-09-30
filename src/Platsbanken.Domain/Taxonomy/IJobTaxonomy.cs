using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Taxonomy;

/// <summary>Port: look up JobTech Taxonomy concepts, for example to turn a municipality name into a ConceptId.</summary>
public interface IJobTaxonomy
{
    /// <summary>All concepts of a type. Implementations may cache.</summary>
    Task<IReadOnlyList<Concept>> GetConceptsAsync(ConceptType type, CancellationToken cancellationToken = default);
}
