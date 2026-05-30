using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyracare.Auth.Common.Http;
using Vyracare.Auth.Features.Auth.FirstAccessCheck;
using Vyracare.Auth.Features.Auth.FirstAccessSetPassword;
using Vyracare.Auth.Features.Auth.ForgotPassword;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Register;

namespace Vyracare.Auth.Features.Auth;

[ApiController]
[Route("api/auth")]
/// <summary>
/// Exp?e os endpoints HTTP desta feature e delega o processamento para os handlers da aplica??o.
/// </summary>
public sealed class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
/// <summary>
/// Executa a responsabilidade associada a r eg is te r.
/// </summary>
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, value => CreatedAtAction(nameof(Register), new { id = value.Id }, new { message = value.Message }));
    }

    [AllowAnonymous]
    [HttpPost("first-access/check")]
/// <summary>
/// Executa a responsabilidade associada a c he ck fi rs ta cc es s.
/// </summary>
    public async Task<IActionResult> CheckFirstAccess(
        [FromBody] FirstAccessCheckRequest request,
        [FromServices] FirstAccessCheckHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, Ok);
    }

    [AllowAnonymous]
    [HttpPost("first-access/set-password")]
/// <summary>
/// Executa a responsabilidade associada a s et fi rs ta cc es sp as sw or d.
/// </summary>
    public async Task<IActionResult> SetFirstAccessPassword(
        [FromBody] FirstAccessSetPasswordRequest request,
        [FromServices] FirstAccessSetPasswordHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, Ok);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
/// <summary>
/// Executa a responsabilidade associada a f or go tp as sw or d.
/// </summary>
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] ForgotPasswordHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, Ok);
    }

    [AllowAnonymous]
    [HttpPost("login")]
/// <summary>
/// Executa a responsabilidade associada a l og in.
/// </summary>
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, Ok);
    }
}
