namespace Platsbanken.Domain.Ads;

/// <summary>Identifier of a job ad in Platsbanken.</summary>
public readonly record struct JobAdId
{
    /// <summary>The raw ad id, for example <c>31529272</c>.</summary>
    public string Value { get; }

    /// <summary>Creates a job ad id.</summary>
    /// <param name="value">The raw id. Must not be null or blank.</param>
    public JobAdId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
