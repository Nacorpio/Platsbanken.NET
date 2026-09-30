namespace Platsbanken.Domain.Ads;

/// <summary>The employer behind a job ad.</summary>
/// <param name="Name">Employer name.</param>
/// <param name="OrganizationNumber">Swedish organization number, when published.</param>
/// <param name="Workplace">Name of the specific workplace, when different from the employer.</param>
/// <param name="Url">Employer web page.</param>
/// <param name="Email">Contact e-mail address.</param>
/// <param name="PhoneNumber">Contact phone number.</param>
public sealed record Employer(
    string? Name,
    string? OrganizationNumber,
    string? Workplace,
    string? Url,
    string? Email,
    string? PhoneNumber);
