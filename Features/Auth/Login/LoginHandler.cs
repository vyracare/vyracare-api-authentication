using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Implementa o caso de uso correspondente a esta feature.
/// </summary>
public sealed class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

/// <summary>
/// Inicializa uma nova instância de LoginHandler.
/// </summary>
    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

/// <summary>
/// Executa o caso de uso e devolve o resultado padronizado da operação.
/// </summary>
    public async Task<UseCaseResult<LoginResponse>> HandleAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return UseCaseResult<LoginResponse>.Failure(UseCaseErrorType.Validation, "Email and password are required");
        }

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim());
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return UseCaseResult<LoginResponse>.Failure(UseCaseErrorType.Unauthorized, "Invalid credentials");
        }

        var token = _jwtTokenGenerator.Generate(user);
        return UseCaseResult<LoginResponse>.Success(new LoginResponse(token));
    }
}
