namespace Vyracare.Auth.Common.Time;

/// <summary>
/// Define um contrato para fornecer a data e hora atual da aplicação.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
