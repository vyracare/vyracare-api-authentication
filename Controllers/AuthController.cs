using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;         
using Microsoft.AspNetCore.Mvc;
using Vyracare.Auth.Models;
using Vyracare.Auth.Services;

namespace Vyracare.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _config;

    public AuthController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var exists = await _userService.GetByEmailAsync(req.Email); // ajuste de nome
        if (exists != null) return Conflict(new { message = "User already exists" });

        var user = new UserModel { Email = req.Email, FullName = req.FullName };
        user.PasswordHash = _userService.HashPassword(req.Password);
        await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(Register), new { id = user.Id }, new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userService.GetByEmailAsync(req.Email); // ajuste de nome
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });
        if (!_userService.VerifyPassword(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _userService.GenerateJwt(user, _config);
        return Ok(new { token });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (sub == null) return Unauthorized();
        var user = await _userService.GetByIdAsync(sub);
        if (user == null) return NotFound();
        return Ok(new { user.Email, user.FullName, user.Id });
    }
}
