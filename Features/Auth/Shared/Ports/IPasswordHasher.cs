namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define o contrato para geração e verificação de hashes usados pela aplicação.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}
