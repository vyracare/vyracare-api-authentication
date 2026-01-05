using MongoDB.Driver;
using Vyracare.Auth.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Vyracare.Auth.Services;

public class UserService
{
    private readonly IMongoCollection<UserModel> _users;
    private readonly IConfiguration _config;

    public UserService(IMongoDatabase db, IConfiguration config)
    {
        _users = db.GetCollection<UserModel>("users");
        _config = config;
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<UserModel?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public Task CreateAsync(UserModel user)
    {
        user.CreatedAt = DateTime.UtcNow;
        return _users.InsertOneAsync(user);
    }

    public string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }

    public string GenerateJwt(UserModel user, IConfiguration config)
    {
        var key = config["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT key missing");
        var issuer = config["Jwt:Issuer"] ?? "vyracare-auth";
        var audience = config["Jwt:Audience"] ?? "vyracare-client";
        var expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");

        if (string.IsNullOrEmpty(user.Id))
            throw new ArgumentException("Id do usuário inválido para criação da Claim.");

        if (string.IsNullOrEmpty(user.Email))
            throw new ArgumentException("Email inválido para criação da Claim.");

        var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.FullName));
        }

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiryMinutes), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
