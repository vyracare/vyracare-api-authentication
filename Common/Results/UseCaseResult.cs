namespace Vyracare.Auth.Common.Results;

/// <summary>
/// Representa os tipos de erro padronizados usados pelos casos de uso da aplicação.
/// Essa enumeração permite mapear falhas de domínio para respostas HTTP consistentes.
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
/// Representa o envelope padrão retornado pelos handlers.
/// O objetivo é separar a regra de negócio do protocolo HTTP, devolvendo sempre um resultado
/// que possa ser interpretado depois pelo controller.
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
    /// Quando esse valor é verdadeiro, o conteúdo esperado está em <see cref="Value"/>.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Contém o valor útil produzido pelo caso de uso quando a operação é bem-sucedida.
    /// Em falha, essa propriedade tende a ficar nula ou com o valor padrão do tipo.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Identifica a categoria da falha quando o caso de uso não é bem-sucedido.
    /// Esse valor é usado pelo controller para decidir o status HTTP adequado.
    /// </summary>
    public UseCaseErrorType ErrorType { get; }

    /// <summary>
    /// Contém a mensagem associada ao resultado.
    /// Em erro, a mensagem explica a causa da falha; em sucesso, normalmente fica vazia.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Cria um resultado de sucesso com o valor produzido pelo caso de uso.
    /// </summary>
    /// <param name="value">Valor que será entregue ao controller e, em seguida, ao cliente.</param>
    /// <returns>Um resultado sem erro e com valor preenchido.</returns>
    public static UseCaseResult<T> Success(T value) => new(true, value, UseCaseErrorType.None, string.Empty);

    /// <summary>
    /// Cria um resultado de falha com categoria e mensagem explícitas.
    /// </summary>
    /// <param name="errorType">Tipo padronizado da falha ocorrida.</param>
    /// <param name="message">Mensagem descritiva da falha.</param>
    /// <returns>Um resultado sem valor útil e marcado como falha.</returns>
    public static UseCaseResult<T> Failure(UseCaseErrorType errorType, string message) =>
        new(false, default, errorType, message);
}
