using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ForgotPassword;

/// <summary>
/// Implementa o caso de uso correspondente a esta feature.
/// </summary>
public sealed class ForgotPasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

/// <summary>
/// Inicializa uma nova instância de ForgotPasswordHandler.
/// </summary>
    public ForgotPasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

/// <summary>
/// Executa o caso de uso e devolve o resultado padronizado da operação.
/// </summary>
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
