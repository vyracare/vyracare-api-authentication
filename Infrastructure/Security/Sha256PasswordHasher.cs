using System.Security.Cryptography;
using System.Text;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Infrastructure.Security;

/// <summary>
/// Implementa o cálculo de hash de senha usando SHA-256.
/// Esta implementação é simples e determinística, servindo como adapter concreto para a porta de hashing.
/// </summary>
public sealed class Sha256PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Converte a senha em texto puro para um hash Base64 usando o algoritmo SHA-256.
    /// </summary>
    /// <param name="password">Senha informada pelo cliente.</param>
    /// <returns>Hash da senha em formato Base64.</returns>
    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verifica se a senha recebida gera o mesmo hash persistido para o usuário.
    /// </summary>
    /// <param name="password">Senha em texto puro recebida na requisição.</param>
    /// <param name="storedHash">Hash persistido em banco para o usuário.</param>
    /// <returns><see langword="true"/> quando os valores coincidem; caso contrário, <see langword="false"/>.</returns>
    public bool Verify(string password, string storedHash)
    {
        return Hash(password) == storedHash;
    }
}
