namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa as configurações necessárias para emitir e validar tokens JWT na API de autenticação.
/// Esses valores podem vir de <c>appsettings.json</c>, variáveis de ambiente ou AWS Secrets Manager.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Nome da seção de configuração usada para popular esta classe.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Obtém ou define a chave simétrica usada para assinar os tokens gerados pela API.
    /// Em produção, esse valor deve vir de uma fonte segura e nunca do código-fonte.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o emissor que será gravado no token e validado pelas APIs consumidoras.
    /// Esse valor ajuda a garantir que o token foi emitido pela aplicação correta.
    /// </summary>
    public string Issuer { get; set; } = "vyracare-auth";

    /// <summary>
    /// Obtém ou define o público esperado para o token.
    /// Esse valor impede que um token emitido para outro contexto seja aceito por engano.
    /// </summary>
    public string Audience { get; set; } = "vyracare-client";

    /// <summary>
    /// Obtém ou define a quantidade de minutos de validade do token após sua emissão.
    /// Esse valor é usado para calcular a expiração do JWT.
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;
}
