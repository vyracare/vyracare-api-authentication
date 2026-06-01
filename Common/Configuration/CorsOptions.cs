namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa as opções de configuração carregadas da aplicação.
/// </summary>
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

/// <summary>
/// Obtém ou define as origens permitidas na política de CORS.
/// </summary>
    public string AllowedOrigins { get; set; } = "*";
}
