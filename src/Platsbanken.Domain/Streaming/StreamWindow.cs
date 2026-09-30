using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Streaming;

/// <summary>Time window and filters for a JobStream request.</summary>
public sealed record StreamWindow
{
    public StreamWindow(DateTimeOffset updatedAfter, DateTimeOffset? updatedBefore = null)
    {
        if (updatedBefore is { } before && before <= updatedAfter)
        {
            throw new ArgumentException("UpdatedBefore must be later than UpdatedAfter.", nameof(updatedBefore));
        }

        UpdatedAfter = updatedAfter;
        UpdatedBefore = updatedBefore;
    }

    public DateTimeOffset UpdatedAfter { get; }
    public DateTimeOffset? UpdatedBefore { get; }
    public IReadOnlyList<ConceptId> OccupationConceptIds { get; init; } = [];
    public IReadOnlyList<ConceptId> LocationConceptIds { get; init; } = [];
}
