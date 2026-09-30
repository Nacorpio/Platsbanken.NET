using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Search;

/// <summary>Filters and paging for a job ad search.</summary>
public sealed record JobSearchCriteria
{
    /// <summary>Largest page size the API accepts.</summary>
    public const int MaxLimit = 100;

    /// <summary>Largest offset the API accepts. Narrow the criteria or use JobStream beyond this.</summary>
    public const int MaxOffset = 2000;

    /// <summary>Free-text query.</summary>
    public string? Query { get; init; }

    /// <summary>Occupation-name concept ids.</summary>
    public IReadOnlyList<ConceptId> Occupations { get; init; } = [];

    /// <summary>Occupation-group concept ids.</summary>
    public IReadOnlyList<ConceptId> OccupationGroups { get; init; } = [];

    /// <summary>Occupation-field concept ids.</summary>
    public IReadOnlyList<ConceptId> OccupationFields { get; init; } = [];

    /// <summary>Municipality concept ids.</summary>
    public IReadOnlyList<ConceptId> Municipalities { get; init; } = [];

    /// <summary>Region concept ids.</summary>
    public IReadOnlyList<ConceptId> Regions { get; init; } = [];

    /// <summary>Employer name filter.</summary>
    public string? Employer { get; init; }

    /// <summary>Only ads published after this instant.</summary>
    public DateTimeOffset? PublishedAfter { get; init; }

    /// <summary>True for remote-work ads only, false to exclude them, null for both.</summary>
    public bool? Remote { get; init; }

    /// <summary>Index of the first hit, from 0 to <see cref="MaxOffset"/>.</summary>
    public int Offset { get; init; }

    /// <summary>Page size, from 1 to <see cref="MaxLimit"/>.</summary>
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
