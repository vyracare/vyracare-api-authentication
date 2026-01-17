using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;         
using Microsoft.AspNetCore.Mvc;
using Vyracare.Auth.DTOS;
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
        var exists = await _userService.GetByEmailAsync(req.Email);
        if (exists != null) return Conflict(new { message = "User already exists" });

        var user = new UserModel
        {
            Email = req.Email,
            FullName = req.FullName,
            Role = req.Role,
            Department = req.Department,
            Phone = req.Phone,
            AccessLevel = req.AccessLevel,
            Active = req.Active ?? true
        };

        if (!string.IsNullOrWhiteSpace(req.Password))
        {
            user.PasswordHash = _userService.HashPassword(req.Password);
        }
        else
        {
            user.PasswordHash = string.Empty;
        }

        await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(Register), new { id = user.Id }, new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userService.GetByEmailAsync(req.Email);
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });
        if (!_userService.VerifyPassword(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _userService.GenerateJwt(user, _config);
        return Ok(new { token });
    }

}
