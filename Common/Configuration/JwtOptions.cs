namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa uma configura??o tipada lida do appsettings ou das vari?veis de ambiente.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

/// <summary>
/// Obt?m ou define k ey.
/// </summary>
    public string Key { get; set; } = string.Empty;
/// <summary>
/// Obt?m ou define i ss ue r.
/// </summary>
    public string Issuer { get; set; } = "vyracare-auth";
/// <summary>
/// Obt?m ou define a ud ie nc e.
/// </summary>
    public string Audience { get; set; } = "vyracare-client";
/// <summary>
/// Obt?m ou define e xp ir ym in ut es.
/// </summary>
    public int ExpiryMinutes { get; set; } = 60;
}
