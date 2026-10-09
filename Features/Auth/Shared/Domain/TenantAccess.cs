namespace Vyracare.Auth.Features.Auth.Shared.Domain;

public sealed record TenantAccess(
    string TenantId,
    string MembershipId,
    string Role,
    string Status,
    DateTime TrialStartsAtUtc,
    DateTime TrialEndsAtUtc);

