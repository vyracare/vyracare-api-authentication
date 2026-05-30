namespace Vyracare.Auth.Features.Auth.Shared.Ports;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}
