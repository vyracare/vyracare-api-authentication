namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa as configurações responsáveis por controlar quais origens podem consumir a API.
/// </summary>
public sealed class CorsOptions
{
    /// <summary>
    /// Nome da seção de configuração usada para popular esta classe.
    /// </summary>
    public const string SectionName = "Cors";

    /// <summary>
    /// Obtém ou define as origens permitidas na política de CORS.
    /// Pode receber um único domínio, vários domínios separados por vírgula ou <c>*</c>.
    /// </summary>
    public string AllowedOrigins { get; set; } = "*";
}
