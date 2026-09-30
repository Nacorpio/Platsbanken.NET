namespace Platsbanken.Domain.Taxonomy;

/// <summary>A JobTech Taxonomy concept type, such as region or occupation-name.</summary>
public readonly record struct ConceptType
{
    public static readonly ConceptType OccupationName = new("occupation-name");
    public static readonly ConceptType OccupationField = new("occupation-field");
    public static readonly ConceptType OccupationCollection = new("occupation-collection");
    public static readonly ConceptType Municipality = new("municipality");
    public static readonly ConceptType Region = new("region");
    public static readonly ConceptType Country = new("country");
    public static readonly ConceptType Skill = new("skill");
    public static readonly ConceptType Language = new("language");
    public static readonly ConceptType EmploymentType = new("employment-type");
    public static readonly ConceptType DrivingLicence = new("driving-licence");

    public string Value { get; }

    public ConceptType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public override string ToString() => Value;
}
