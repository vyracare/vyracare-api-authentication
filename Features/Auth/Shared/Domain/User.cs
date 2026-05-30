namespace Vyracare.Auth.Features.Auth.Shared.Domain;

/// <summary>
/// Representa uma parte da arquitetura desta API.
/// </summary>
public sealed class User
{
/// <summary>
/// Identificador do registro ou do recurso processado.
/// </summary>
    public string? Id { get; set; }
/// <summary>
/// Obt?m ou define e ma il.
/// </summary>
    public string Email { get; set; } = string.Empty;
/// <summary>
/// Obt?m ou define f ul ln am e.
/// </summary>
    public string? FullName { get; set; }
/// <summary>
/// Obt?m ou define r ol e.
/// </summary>
    public string? Role { get; set; }
/// <summary>
/// Obt?m ou define d ep ar tm en t.
/// </summary>
    public string? Department { get; set; }
/// <summary>
/// Obt?m ou define p ho ne.
/// </summary>
    public string? Phone { get; set; }
/// <summary>
/// Obt?m ou define a cc es sl ev el.
/// </summary>
    public string? AccessLevel { get; set; }
/// <summary>
/// Obt?m ou define a ct iv e.
/// </summary>
    public bool Active { get; set; } = true;
/// <summary>
/// Obt?m ou define p as sw or dh as h.
/// </summary>
    public string PasswordHash { get; set; } = string.Empty;
/// <summary>
/// Data de cria??o do registro.
/// </summary>
    public DateTime CreatedAt { get; set; }
}
