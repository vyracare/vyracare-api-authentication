using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.FirstAccessSetPassword;

/// <summary>
/// Implementa o caso de uso que define a senha inicial de um usuário no primeiro acesso.
/// O fluxo só é permitido quando o usuário existe e ainda não possui senha persistida.
/// </summary>
public sealed class FirstAccessSetPasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Inicializa uma nova instância do handler responsável por gravar a senha inicial.
    /// </summary>
    public FirstAccessSetPasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Executa o fluxo de definição de senha do primeiro acesso.
    /// O método valida campos obrigatórios, garante um tamanho mínimo para a senha, verifica
    /// se o usuário existe e impede a redefinição quando a senha já foi cadastrada anteriormente.
    /// </summary>
    /// <param name="request">E-mail do usuário e senha inicial escolhida.</param>
    /// <returns>Mensagem de sucesso ou falha padronizada para o fluxo.</returns>
    public async Task<UseCaseResult<MessageResponse>> HandleAsync(FirstAccessSetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Validation, "Email and password are required");
        }

        if (request.Password.Length < 6)
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Validation, "Password must be at least 6 characters");
        }

        var email = request.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.NotFound, "User not found");
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Conflict, "Password already set");
        }

        var updated = await _userRepository.SetPasswordIfEmptyAsync(email, _passwordHasher.Hash(request.Password));
        if (!updated)
        {
            return UseCaseResult<MessageResponse>.Failure(UseCaseErrorType.Conflict, "Password already set");
        }

        return UseCaseResult<MessageResponse>.Success(new MessageResponse("Password set"));
    }
}
