using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Implementa o caso de uso de autenticação por e-mail e senha.
/// A responsabilidade desta classe é validar os dados recebidos, localizar o usuário,
/// conferir a senha e emitir um token quando tudo estiver correto.
/// </summary>
public sealed class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITenancyProvisioner? _tenancy;

    /// <summary>
    /// Inicializa uma nova instância do handler de login com as dependências necessárias
    /// para consultar usuários, validar senha e gerar token.
    /// </summary>
    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ITenancyProvisioner? tenancy = null)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _tenancy = tenancy;
    }

    /// <summary>
    /// Executa o fluxo completo de login.
    /// Primeiro valida campos obrigatórios, depois carrega o usuário por e-mail, compara a senha
    /// informada com o hash persistido e, por fim, gera o JWT de acesso.
    /// </summary>
    /// <param name="request">Credenciais enviadas pelo cliente.</param>
    /// <returns>Token de autenticação em caso de sucesso ou falha padronizada em caso de erro.</returns>
    public async Task<UseCaseResult<LoginResponse>> HandleAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return UseCaseResult<LoginResponse>.Failure(UseCaseErrorType.Validation, "Email and password are required");
        }

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim());
        if (user is null || !user.Active || string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return UseCaseResult<LoginResponse>.Failure(UseCaseErrorType.Unauthorized, "Invalid credentials");
        }

        if (user.TenantAccess is null && !string.IsNullOrWhiteSpace(user.Id) && _tenancy is not null)
        {
            try
            {
                var memberships = await _tenancy.GetMembershipsAsync(user.Id);
                if (memberships.Count == 1)
                {
                    user.TenantAccess = memberships.Single();
                    await _userRepository.SetTenantAccessAsync(user.Id, user.TenantAccess);
                }
            }
            catch
            {
                // O login legado continua valido; o shell conduz a recuperacao quando nao houver tenant.
            }
        }

        var token = _jwtTokenGenerator.Generate(user);
        return UseCaseResult<LoginResponse>.Success(new LoginResponse(token));
    }
}
