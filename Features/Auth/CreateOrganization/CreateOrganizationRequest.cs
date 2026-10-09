namespace Vyracare.Auth.Features.Auth.CreateOrganization;

public sealed record CreateOrganizationRequest(string LegalName, string? TradeName, string? Document);

public sealed record CreateOrganizationResponse(
    string Token,
    string TenantId,
    string MembershipId,
    DateTime TrialEndsAtUtc);

