namespace Vyracare.Auth.Common.Time;

public interface IClock
{
    DateTime UtcNow { get; }
}
