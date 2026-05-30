namespace Vyracare.Auth.Common.Results;

/// <summary>
/// Define um conjunto de valores conhecidos usados pela aplica??o.
/// </summary>
public enum UseCaseErrorType
{
    None,
    Validation,
    Conflict,
    NotFound,
    Unauthorized
}

/// <summary>
/// Representa uma parte da arquitetura desta API.
/// </summary>
public sealed class UseCaseResult<T>
{
    private UseCaseResult(bool isSuccess, T? value, UseCaseErrorType errorType, string message)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorType = errorType;
        Message = message;
    }

/// <summary>
/// Indica se a opera??o foi conclu?da com sucesso.
/// </summary>
    public bool IsSuccess { get; }
/// <summary>
/// Armazena o valor retornado quando a opera??o ? conclu?da com sucesso.
/// </summary>
    public T? Value { get; }
/// <summary>
/// Indica a categoria do erro retornado pela opera??o.
/// </summary>
    public UseCaseErrorType ErrorType { get; }
/// <summary>
/// Cont?m a mensagem associada ao resultado da opera??o.
/// </summary>
    public string Message { get; }

/// <summary>
/// Executa a responsabilidade associada a s uc ce ss.
/// </summary>
    public static UseCaseResult<T> Success(T value) => new(true, value, UseCaseErrorType.None, string.Empty);

/// <summary>
/// Executa a responsabilidade associada a f ai lu re.
/// </summary>
    public static UseCaseResult<T> Failure(UseCaseErrorType errorType, string message) =>
        new(false, default, errorType, message);
}
