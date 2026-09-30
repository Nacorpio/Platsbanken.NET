namespace Platsbanken.Domain.Ads;

/// <summary>Identifier of a JobTech Taxonomy concept (occupation, municipality, region and so on).</summary>
public readonly record struct ConceptId
{
    public string Value { get; }

    public ConceptId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public override string ToString() => Value;
}
