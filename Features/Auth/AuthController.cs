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
/// Expõe os endpoints HTTP relacionados ao domínio de autenticação.
/// O controller apenas recebe a requisição, resolve o handler apropriado via DI e converte o
/// resultado da regra de negócio em resposta HTTP.
/// </summary>
public sealed class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    /// <summary>
    /// Recebe os dados do novo usuário e delega o cadastro ao handler de registro.
    /// Em caso de sucesso, devolve <c>201 Created</c> com a mensagem e o identificador gerado.
    /// </summary>
    /// <param name="request">Dados necessários para criar o usuário inicial na base.</param>
    /// <param name="handler">Caso de uso responsável por aplicar as regras de cadastro.</param>
    /// <returns>Resposta HTTP correspondente ao resultado do cadastro.</returns>
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
    /// Verifica se o usuário informado existe e se ainda está apto a definir a senha do primeiro acesso.
    /// </summary>
    /// <param name="request">Dados mínimos para localizar o usuário pelo e-mail.</param>
    /// <param name="handler">Caso de uso responsável pela verificação de primeiro acesso.</param>
    /// <returns>Resposta com o status do primeiro acesso para o e-mail informado.</returns>
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
    /// Permite que um usuário sem senha cadastrada defina a credencial inicial de acesso.
    /// </summary>
    /// <param name="request">E-mail do usuário e senha que deverá ser gravada pela primeira vez.</param>
    /// <param name="handler">Caso de uso responsável por validar e persistir a senha inicial.</param>
    /// <returns>Resposta HTTP contendo o resultado da operação.</returns>
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
    /// Atualiza a senha de um usuário existente em um fluxo de recuperação de acesso.
    /// </summary>
    /// <param name="request">E-mail do usuário e nova senha definida no fluxo de recuperação.</param>
    /// <param name="handler">Caso de uso responsável por regravar a senha.</param>
    /// <returns>Resposta HTTP indicando sucesso ou falha da recuperação.</returns>
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
    /// Valida as credenciais enviadas pelo cliente e devolve um token JWT quando a autenticação é bem-sucedida.
    /// </summary>
    /// <param name="request">E-mail e senha informados pelo usuário.</param>
    /// <param name="handler">Caso de uso responsável por validar credenciais e emitir o token.</param>
    /// <returns>Resposta HTTP com token em caso de sucesso ou erro em caso de falha.</returns>
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return this.ToActionResult(result, Ok);
    }
}
