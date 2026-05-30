namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa uma configura??o tipada lida do appsettings ou das vari?veis de ambiente.
/// </summary>
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

/// <summary>
/// Obt?m ou define a ll ow ed or ig in s.
/// </summary>
    public string AllowedOrigins { get; set; } = "*";
}
