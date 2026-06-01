namespace Vyracare.Auth.Features.Auth.Register;

/// <summary>
/// Representa os dados necessários para cadastrar um usuário na plataforma.
/// Além do e-mail e da senha opcional, o contrato já carrega informações de perfil e acesso.
/// </summary>
/// <param name="Email">E-mail único do usuário dentro da plataforma.</param>
/// <param name="Password">Senha inicial opcional. Quando vazia, o usuário pode concluir o primeiro acesso depois.</param>
/// <param name="FullName">Nome completo exibido na plataforma.</param>
/// <param name="Role">Papel ou cargo do usuário dentro da organização.</param>
/// <param name="Department">Departamento ao qual o usuário pertence.</param>
/// <param name="Phone">Telefone de contato associado ao usuário.</param>
/// <param name="AccessLevel">Nível de acesso textual usado pela aplicação.</param>
/// <param name="Active">Indicador opcional de usuário ativo; quando omitido, o cadastro assume ativo.</param>
public sealed record RegisterRequest(
    string Email,
    string? Password,
    string? FullName,
    string? Role,
    string? Department,
    string? Phone,
    string? AccessLevel,
    bool? Active
);
