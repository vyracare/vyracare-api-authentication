using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define a porta de persistência usada pelos casos de uso de autenticação.
/// Os handlers dependem desta interface para não conhecer detalhes de MongoDB ou qualquer outra tecnologia.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Recupera um usuário a partir do e-mail informado.
    /// </summary>
    /// <param name="email">E-mail usado como critério de busca.</param>
    /// <returns>Usuário encontrado ou <see langword="null"/> quando não existir.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Persiste um novo usuário na base.
    /// </summary>
    /// <param name="user">Entidade de domínio pronta para gravação.</param>
    /// <returns>A própria entidade com eventuais ajustes feitos pelo repositório, como o identificador.</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Define a senha do usuário somente se ele ainda não possuir um hash persistido.
    /// Esse método protege o fluxo de primeiro acesso contra sobrescrita indevida.
    /// </summary>
    /// <param name="email">E-mail do usuário que receberá a senha.</param>
    /// <param name="passwordHash">Hash calculado da nova senha.</param>
    /// <returns><see langword="true"/> quando a senha foi gravada; caso contrário, <see langword="false"/>.</returns>
    Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash);

    /// <summary>
    /// Atualiza a senha de um usuário existente independentemente de ele já possuir senha cadastrada.
    /// Esse método é usado no fluxo de recuperação.
    /// </summary>
    /// <param name="email">E-mail do usuário que terá a senha alterada.</param>
    /// <param name="passwordHash">Hash calculado da nova senha.</param>
    /// <returns><see langword="true"/> quando o usuário foi encontrado; caso contrário, <see langword="false"/>.</returns>
    Task<bool> UpdatePasswordAsync(string email, string passwordHash);
}
