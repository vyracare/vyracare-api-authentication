namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Novo estado operacional solicitado para o funcionario.
/// </summary>
public sealed record ChangeEmployeeStatusRequest(bool Active);
