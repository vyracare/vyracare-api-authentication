namespace Vyracare.Auth.Common.Results;

/// <summary>
/// Representa os tipos de erro padronizados retornados pelos casos de uso.
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
/// Representa o componente UseCaseResult<T> da aplicação.
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
/// Indica se a operação foi concluída com sucesso.
/// </summary>
    public bool IsSuccess { get; }
/// <summary>
/// Armazena o valor retornado quando a operação é concluída com sucesso.
/// </summary>
    public T? Value { get; }
/// <summary>
/// Indica a categoria do erro retornado pela operação.
/// </summary>
    public UseCaseErrorType ErrorType { get; }
/// <summary>
/// Contém a mensagem associada ao resultado da operação.
/// </summary>
    public string Message { get; }

/// <summary>
/// Cria um resultado de sucesso com o valor informado.
/// </summary>
    public static UseCaseResult<T> Success(T value) => new(true, value, UseCaseErrorType.None, string.Empty);

/// <summary>
/// Cria um resultado de falha com o tipo de erro e a mensagem informados.
/// </summary>
    public static UseCaseResult<T> Failure(UseCaseErrorType errorType, string message) =>
        new(false, default, errorType, message);
}
