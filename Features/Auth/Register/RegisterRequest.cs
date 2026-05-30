namespace Vyracare.Auth.Features.Auth.Register;

/// <summary>
/// Define o contrato de entrada esperado por este caso de uso.
/// </summary>
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
