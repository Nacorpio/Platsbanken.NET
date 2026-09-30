namespace Platsbanken.Domain.Ads;

/// <summary>Identifier of a job ad in Platsbanken.</summary>
public readonly record struct JobAdId
{
    public string Value { get; }

    public JobAdId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public override string ToString() => Value;
}
