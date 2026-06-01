using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.Register;

/// <summary>
/// Implementa o caso de uso de cadastro de usuário.
/// Esta classe garante que não haja duplicidade por e-mail e normaliza os dados antes de persisti-los.
/// </summary>
public sealed class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClock _clock;

    /// <summary>
    /// Inicializa uma nova instância do handler de registro.
    /// </summary>
    public RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IClock clock)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
    }

    /// <summary>
    /// Executa o fluxo de cadastro.
    /// O método valida o e-mail, impede conflitos por duplicidade, monta a entidade de domínio
    /// com dados normalizados e persiste o usuário na base.
    /// </summary>
    /// <param name="request">Dados recebidos do cliente para criação do usuário.</param>
    /// <returns>Identificador e mensagem de sucesso, ou uma falha padronizada.</returns>
    public async Task<UseCaseResult<RegisterResponse>> HandleAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Validation, "Email is required");
        }

        var email = request.Email.Trim();
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
        {
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Conflict, "User already exists");
        }

        var timestamp = _clock.UtcNow;
        var user = new Shared.Domain.User
        {
            Email = email,
            FullName = request.FullName?.Trim(),
            Role = request.Role?.Trim(),
            Department = request.Department?.Trim(),
            Phone = request.Phone?.Trim(),
            AccessLevel = request.AccessLevel?.Trim(),
            Active = request.Active ?? true,
            PasswordHash = string.IsNullOrWhiteSpace(request.Password) ? string.Empty : _passwordHasher.Hash(request.Password),
            CreatedAt = timestamp
        };

        var created = await _userRepository.AddAsync(user);
        return UseCaseResult<RegisterResponse>.Success(new RegisterResponse(created.Id ?? string.Empty, "User created"));
    }
}
