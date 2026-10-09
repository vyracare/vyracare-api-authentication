namespace Vyracare.Auth.Common.Security;

/// <summary>
/// Centraliza os nomes de claims proprietarias emitidas e consumidas pela API.
/// </summary>
public static class JwtClaimNames
{
    /// <summary>
    /// Nivel de acesso usado como fonte de autorizacao por role.
    /// </summary>
    public const string AccessLevel = "access_level";
    public const string TenantId = "tenant_id";
    public const string MembershipId = "membership_id";
    public const string TenantRole = "tenant_role";
    public const string Plan = "plan";
}
