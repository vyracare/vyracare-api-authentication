using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Implementa o caso de uso correspondente a esta feature.
/// </summary>
public sealed class FirstAccessCheckHandler
{
    private readonly IUserRepository _userRepository;

/// <summary>
/// Inicializa uma nova instância de FirstAccessCheckHandler.
/// </summary>
    public FirstAccessCheckHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

/// <summary>
/// Executa o caso de uso e devolve o resultado padronizado da operação.
/// </summary>
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
