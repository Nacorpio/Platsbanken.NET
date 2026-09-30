using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

public sealed record JobSearchCriteria
{
    public const int MaxLimit = 100;
    public const int MaxOffset = 2000;

    public string? Query { get; init; }
    public IReadOnlyList<ConceptId> Occupations { get; init; } = [];
    public IReadOnlyList<ConceptId> OccupationGroups { get; init; } = [];
    public IReadOnlyList<ConceptId> OccupationFields { get; init; } = [];
    public IReadOnlyList<ConceptId> Municipalities { get; init; } = [];
    public IReadOnlyList<ConceptId> Regions { get; init; } = [];
    public string? Employer { get; init; }
    public DateTimeOffset? PublishedAfter { get; init; }
    public bool? Remote { get; init; }
    public int Offset { get; init; }
    public int Limit { get; init; } = 10;

    /// <summary>Throws when paging bounds violate what the API accepts.</summary>
    public void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Offset);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Offset, MaxOffset);
        ArgumentOutOfRangeException.ThrowIfLessThan(Limit, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Limit, MaxLimit);
    }
}
