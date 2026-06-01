using Vyracare.Auth.Common.Time;

namespace Vyracare.Auth.Infrastructure.Time;

/// <summary>
/// Implementa o relógio padrão da aplicação usando a hora UTC do sistema operacional.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <summary>
    /// Obtém a data e hora atual em UTC a partir do sistema.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}
