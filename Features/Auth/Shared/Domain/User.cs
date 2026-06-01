namespace Vyracare.Auth.Features.Auth.Shared.Domain;

/// <summary>
/// Representa a entidade de domínio de usuário usada pelos fluxos de autenticação.
/// Ela concentra os dados necessários para cadastro, login, primeiro acesso e recuperação de senha.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Obtém ou define o identificador persistido do usuário.
    /// Esse valor costuma ser preenchido pelo repositório após a gravação.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Obtém ou define o e-mail único do usuário.
    /// Esse campo é a chave principal de busca nos fluxos desta API.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome completo usado para identificação na plataforma.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Obtém ou define o papel ou cargo do usuário.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Obtém ou define o departamento associado ao usuário.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Obtém ou define o telefone de contato do usuário.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Obtém ou define o nível de acesso textual do usuário.
    /// </summary>
    public string? AccessLevel { get; set; }

    /// <summary>
    /// Obtém ou define se o usuário está ativo e apto a utilizar a plataforma.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Obtém ou define o hash da senha persistido para o usuário.
    /// Quando vazio, a API interpreta que o fluxo de primeiro acesso ainda não foi concluído.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Obtém ou define a data de criação do usuário em UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
