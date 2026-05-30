using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Implementa a regra de neg?cio do caso de uso representado por esta pasta.
/// </summary>
public sealed class FirstAccessCheckHandler
{
    private readonly IUserRepository _userRepository;

/// <summary>
/// Inicializa uma nova inst?ncia de FirstAccessCheckHandler.
/// </summary>
    public FirstAccessCheckHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

/// <summary>
/// Executa o caso de uso e devolve o resultado padronizado da opera??o.
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
