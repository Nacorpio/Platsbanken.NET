namespace Platsbanken.Domain.Ads;

public sealed record Employer(
    string? Name,
    string? OrganizationNumber,
    string? Workplace,
    string? Url,
    string? Email,
    string? PhoneNumber);
