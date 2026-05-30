namespace Vyracare.Auth.Common.Results;

public enum UseCaseErrorType
{
    None,
    Validation,
    Conflict,
    NotFound,
    Unauthorized
}

public sealed class UseCaseResult<T>
{
    private UseCaseResult(bool isSuccess, T? value, UseCaseErrorType errorType, string message)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorType = errorType;
        Message = message;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public UseCaseErrorType ErrorType { get; }
    public string Message { get; }

    public static UseCaseResult<T> Success(T value) => new(true, value, UseCaseErrorType.None, string.Empty);

    public static UseCaseResult<T> Failure(UseCaseErrorType errorType, string message) =>
        new(false, default, errorType, message);
}
