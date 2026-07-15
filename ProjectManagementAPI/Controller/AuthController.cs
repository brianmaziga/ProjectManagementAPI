using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Services;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string _connectionString;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthController(IConfiguration config, IEmailService emailService)
    {
        _config = config;
        _connectionString = config.GetConnectionString("DefaultConnection")!;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QueryFirstOrDefaultAsync<User>(
            @"
            SELECT
                id AS Id,
                name AS Name,
                email AS Email,
                password AS Password,
                status AS Status,
                role AS Role,
                created_at AS CreatedAt
            FROM users
            WHERE email = @Email
            AND password = @Password
            ",
            new { dto.Email, dto.Password }
        );

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (user.Status != "active")
        {
            return Unauthorized(new { message = "Account is not active" });
        }

        var token = GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role
            }
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        using var connection = new SqlConnection(_connectionString);

        var existing = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id AS Id FROM users WHERE email = @Email",
            new { dto.Email }
        );

        if (existing != null)
        {
            return BadRequest(new { message = "An account with this email already exists" });
        }

        await connection.ExecuteAsync(
            @"INSERT INTO users (name, email, password, role, status, created_at)
              VALUES (@Name, @Email, @Password, 'member', 'blocked', GETDATE())",
            new { dto.Name, dto.Email, dto.Password }
        );

        try
        {
            await _emailService.SendEmailAsync(
                dto.Email,
                "Welcome to Project Management Tool",
                $"<h2>Welcome, {dto.Name}!</h2><p>Your account has been created successfully. An administrator will review and activate your account shortly.</p>"
            );
        }
        catch
        {
            // Don't fail registration if the email fails to send
        }

        return Ok(new { message = "Account created successfully" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id AS Id, email AS Email, name AS Name FROM users WHERE email = @Email",
            new { dto.Email }
        );

        if (user == null)
        {
            return Ok(new { message = "If an account exists for that email, a reset code has been sent." });
        }

        var code = new Random().Next(100000, 999999).ToString();

        await connection.ExecuteAsync(
            @"INSERT INTO password_reset_codes (user_id, code, expires_at)
              VALUES (@UserId, @Code, @ExpiresAt)",
            new { UserId = user.Id, Code = code, ExpiresAt = DateTime.UtcNow.AddMinutes(15) }
        );

        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Your Password Reset Code",
                $"<h2>Password Reset</h2><p>Hi {user.Name},</p><p>Your reset code is:</p><h1>{code}</h1><p>This code expires in 15 minutes.</p>"
            );
        }
        catch
        {
            return StatusCode(500, new { message = "Unable to send reset email. Please try again later." });
        }

        return Ok(new { message = "If an account exists for that email, a reset code has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id AS Id FROM users WHERE email = @Email",
            new { dto.Email }
        );

        if (user == null)
        {
            return BadRequest(new { message = "Invalid or expired code" });
        }

        var validCode = await connection.QueryFirstOrDefaultAsync<int?>(
            @"SELECT id FROM password_reset_codes
              WHERE user_id = @UserId AND code = @Code AND used = 0 AND expires_at > GETUTCDATE()",
            new { UserId = user.Id, dto.Code }
        );

        if (validCode == null)
        {
            return BadRequest(new { message = "Invalid or expired code" });
        }

        await connection.ExecuteAsync(
            "UPDATE users SET password = @Password WHERE id = @Id",
            new { dto.Password, user.Id }
        );

        await connection.ExecuteAsync(
            "UPDATE password_reset_codes SET used = 1 WHERE id = @Id",
            new { Id = validCode }
        );

        return Ok(new { message = "Password has been reset successfully" });
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}