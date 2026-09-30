namespace Platsbanken.Domain.Taxonomy;

/// <summary>A JobTech Taxonomy concept type, such as region or occupation-name.</summary>
public readonly record struct ConceptType
{
    /// <summary>Specific occupations, for example "Systemutvecklare".</summary>
    public static readonly ConceptType OccupationName = new("occupation-name");

    /// <summary>Broad occupation fields.</summary>
    public static readonly ConceptType OccupationField = new("occupation-field");

    /// <summary>Curated occupation collections.</summary>
    public static readonly ConceptType OccupationCollection = new("occupation-collection");

    /// <summary>Swedish municipalities (kommuner).</summary>
    public static readonly ConceptType Municipality = new("municipality");

    /// <summary>Swedish regions (län) and foreign regions.</summary>
    public static readonly ConceptType Region = new("region");

    /// <summary>Countries.</summary>
    public static readonly ConceptType Country = new("country");

    /// <summary>Skills.</summary>
    public static readonly ConceptType Skill = new("skill");

    /// <summary>Languages.</summary>
    public static readonly ConceptType Language = new("language");

    /// <summary>Employment types.</summary>
    public static readonly ConceptType EmploymentType = new("employment-type");

    /// <summary>Driving licence categories.</summary>
    public static readonly ConceptType DrivingLicence = new("driving-licence");

    /// <summary>The raw taxonomy type name, as the API expects it.</summary>
    public string Value { get; }

    /// <summary>Creates a concept type from its raw taxonomy name.</summary>
    /// <param name="value">For example <c>municipality</c>. Must not be null or blank.</param>
    public ConceptType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
