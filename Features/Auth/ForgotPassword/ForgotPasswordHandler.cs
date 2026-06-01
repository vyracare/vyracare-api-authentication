using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ForgotPassword;

/// <summary>
/// Implementa o caso de uso de recuperação de senha.
/// Diferente do primeiro acesso, aqui a senha pode ser atualizada para um usuário que já existe.
/// </summary>
public sealed class ForgotPasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Inicializa uma nova instância do handler responsável por redefinir a senha.
    /// </summary>
    public ForgotPasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Executa o fluxo de recuperação de senha.
    /// O método valida os campos de entrada, aplica uma regra mínima de tamanho da senha
    /// e tenta atualizar o hash do usuário informado.
    /// </summary>
    /// <param name="request">E-mail do usuário e nova senha escolhida.</param>
    /// <returns>Mensagem de sucesso ou falha padronizada da operação.</returns>
    public async Task<UseCaseResult<MessageResponse>> HandleAsync(ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Validation, "Email and password are required");
        }

        if (request.Password.Length < 6)
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Validation, "Password must be at least 6 characters");
        }

        var updated = await _userRepository.UpdatePasswordAsync(request.Email.Trim(), _passwordHasher.Hash(request.Password));
        if (!updated)
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.NotFound, "User not found");
        }

        return UseCaseResult<MessageResponse>.Success(new MessageResponse("Password updated"));
    }
}
