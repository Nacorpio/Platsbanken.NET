using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Streaming;

/// <summary>Time window and filters for a JobStream request.</summary>
public sealed record StreamWindow
{
    /// <summary>Creates a window.</summary>
    /// <param name="updatedAfter">Include ads changed after this instant.</param>
    /// <param name="updatedBefore">Include ads changed before this instant. Defaults to now on the server.</param>
    /// <exception cref="ArgumentException"><paramref name="updatedBefore"/> is not later than <paramref name="updatedAfter"/>.</exception>
    public StreamWindow(DateTimeOffset updatedAfter, DateTimeOffset? updatedBefore = null)
    {
        if (updatedBefore is { } before && before <= updatedAfter)
        {
            throw new ArgumentException("UpdatedBefore must be later than UpdatedAfter.", nameof(updatedBefore));
        }

        UpdatedAfter = updatedAfter;
        UpdatedBefore = updatedBefore;
    }

    /// <summary>Start of the window (exclusive).</summary>
    public DateTimeOffset UpdatedAfter { get; }

    /// <summary>End of the window, or null for now.</summary>
    public DateTimeOffset? UpdatedBefore { get; }

    /// <summary>Only ads with one of these occupation, group or field concept ids.</summary>
    public IReadOnlyList<ConceptId> OccupationConceptIds { get; init; } = [];

    /// <summary>Only ads with one of these municipality, region or country concept ids.</summary>
    public IReadOnlyList<ConceptId> LocationConceptIds { get; init; } = [];
}
