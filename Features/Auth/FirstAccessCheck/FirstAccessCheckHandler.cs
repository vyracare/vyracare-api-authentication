using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Implementa o caso de uso que verifica se um usuário existe e se ainda pode definir
/// a senha do primeiro acesso.
/// </summary>
public sealed class FirstAccessCheckHandler
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicializa uma nova instância do handler responsável pela verificação de primeiro acesso.
    /// </summary>
    public FirstAccessCheckHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Executa a verificação de primeiro acesso.
    /// Se o usuário não existir, devolve um resultado de sucesso informando inexistência.
    /// Se existir, avalia se o campo de hash de senha está vazio para decidir se ele ainda pode
    /// concluir o fluxo inicial de definição de senha.
    /// </summary>
    /// <param name="request">E-mail a ser consultado.</param>
    /// <returns>Status de existência do usuário e permissão para definir a senha inicial.</returns>
    public async Task<UseCaseResult<FirstAccessCheckResponse>> HandleAsync(FirstAccessCheckRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return UseCaseResult<FirstAccessCheckResponse>.Failure(UseCaseErrorType.Validation, "Email is required");
        }

        var email = request.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return UseCaseResult<FirstAccessCheckResponse>.Success(new FirstAccessCheckResponse(false, false));
        }

        var canSetPassword = string.IsNullOrWhiteSpace(user.PasswordHash);
        return UseCaseResult<FirstAccessCheckResponse>.Success(new FirstAccessCheckResponse(true, canSetPassword));
    }
}
