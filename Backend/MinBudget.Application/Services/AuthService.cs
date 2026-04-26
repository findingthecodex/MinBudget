using MinBudget.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinBudget.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

// Note: ApplicationUser is defined in Infrastructure.Data
// This class just accepts it as a generic type
public class AuthService<TUser> where TUser : IdentityUser
{
    private readonly UserManager<TUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<TUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var user = (TUser)Activator.CreateInstance(typeof(TUser))!;
        user.UserName = request.Email;
        user.Email = request.Email;
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            UserId = user.Id
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            UserId = user.Id
        };
    }

    private string GenerateJwtToken(TUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

