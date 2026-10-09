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
    private readonly ITenancyProvisioner? _tenancyProvisioner;
    private readonly IJwtTokenGenerator? _jwtTokenGenerator;

    /// <summary>
    /// Inicializa uma nova instância do handler de registro.
    /// </summary>
    public RegisterHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IClock clock,
        ITenancyProvisioner? tenancyProvisioner = null,
        IJwtTokenGenerator? jwtTokenGenerator = null)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _tenancyProvisioner = tenancyProvisioner;
        _jwtTokenGenerator = jwtTokenGenerator;
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
        if (request.Organization is null)
        {
            return UseCaseResult<RegisterResponse>.Success(new RegisterResponse(created.Id ?? string.Empty, "User created"));
        }

        if (_tenancyProvisioner is null || _jwtTokenGenerator is null || string.IsNullOrWhiteSpace(created.Id))
        {
            await _userRepository.DeleteAsync(created.Id ?? string.Empty);
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Unavailable, "Tenant provisioning is unavailable");
        }

        if (string.IsNullOrWhiteSpace(request.Organization.LegalName))
        {
            await _userRepository.DeleteAsync(created.Id);
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Validation, "Organization legal name is required");
        }

        Shared.Domain.TenantAccess? tenantAccess = null;
        try
        {
            tenantAccess = await _tenancyProvisioner.ProvisionOwnerAsync(
                created.Id,
                request.Organization,
                $"owner-registration:{created.Id}");
            created.TenantAccess = tenantAccess;
            await _userRepository.SetTenantAccessAsync(created.Id, tenantAccess);
            var token = _jwtTokenGenerator.Generate(created);
            return UseCaseResult<RegisterResponse>.Success(new RegisterResponse(
                created.Id, "User and organization created", token, tenantAccess.TenantId,
                tenantAccess.MembershipId, tenantAccess.TrialEndsAtUtc));
        }
        catch
        {
            if (tenantAccess is not null)
            {
                try { await _tenancyProvisioner.CompensateOwnerAsync(tenantAccess.TenantId, created.Id); }
                catch { /* A reconciliacao operacional trata compensacoes indisponiveis. */ }
            }
            await _userRepository.DeleteAsync(created.Id);
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Unavailable, "Tenant provisioning failed");
        }
    }
}
