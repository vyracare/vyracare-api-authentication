using Microsoft.AspNetCore.Mvc;
using Vyracare.Auth.Common.Results;

namespace Vyracare.Auth.Common.Http;

/// <summary>
/// Centraliza métodos de extensão para converter resultados de casos de uso em respostas HTTP.
/// </summary>
public static class ControllerBaseExtensions
{
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
