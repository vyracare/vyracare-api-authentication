using Microsoft.AspNetCore.Mvc;
using Vyracare.Auth.Common.Results;

namespace Vyracare.Auth.Common.Http;

/// <summary>
/// Centraliza a tradução entre resultados dos casos de uso e respostas HTTP do ASP.NET Core.
/// Dessa forma, os controllers continuam finos e a regra de mapeamento de erros fica em um só lugar.
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// Converte um <see cref="UseCaseResult{T}"/> em <see cref="IActionResult"/>,
    /// aplicando a regra de sucesso ou erro apropriada.
    /// </summary>
    /// <typeparam name="T">Tipo do retorno produzido pelo handler.</typeparam>
    /// <param name="controller">Controller responsável por construir a resposta HTTP final.</param>
    /// <param name="result">Resultado gerado pelo caso de uso.</param>
    /// <param name="onSuccess">Função que monta a resposta de sucesso, como <c>Ok</c> ou <c>Created</c>.</param>
    /// <returns>Uma resposta HTTP coerente com o resultado devolvido pelo handler.</returns>
    public static IActionResult ToActionResult<T>(
        this ControllerBase controller,
        UseCaseResult<T> result,
        Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        var payload = new { message = result.Message };

        return result.ErrorType switch
        {
            UseCaseErrorType.Validation => controller.BadRequest(payload),
            UseCaseErrorType.Conflict => controller.Conflict(payload),
            UseCaseErrorType.NotFound => controller.NotFound(payload),
            UseCaseErrorType.Unauthorized => controller.Unauthorized(payload),
            _ => controller.BadRequest(payload)
        };
    }
}
