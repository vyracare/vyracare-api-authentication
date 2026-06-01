namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define a porta responsável por transformar senhas em hash e validar comparações posteriores.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera um hash seguro a partir da senha em texto puro.
    /// </summary>
    /// <param name="password">Senha recebida do cliente.</param>
    /// <returns>Representação protegida da senha para persistência.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifica se a senha informada corresponde ao hash já persistido.
    /// </summary>
    /// <param name="password">Senha em texto puro informada na autenticação.</param>
    /// <param name="storedHash">Hash persistido para o usuário.</param>
    /// <returns><see langword="true"/> quando a senha corresponde ao hash; caso contrário, <see langword="false"/>.</returns>
    bool Verify(string password, string storedHash);
}
