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
}
