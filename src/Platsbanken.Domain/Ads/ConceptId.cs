namespace Platsbanken.Domain.Ads;

/// <summary>Identifier of a JobTech Taxonomy concept (occupation, municipality, region and so on).</summary>
public readonly record struct ConceptId
{
    /// <summary>The raw concept id, for example <c>PVZL_BQT_XtL</c>.</summary>
    public string Value { get; }

    /// <summary>Creates a concept id.</summary>
    /// <param name="value">The raw id. Must not be null or blank.</param>
    public ConceptId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
