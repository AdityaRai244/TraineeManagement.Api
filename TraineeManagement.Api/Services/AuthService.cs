
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Helpers;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _database;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext database, IConfiguration config,  ILogger<AuthService> logger)
    {
        _database = database;
        _config = config;
        _logger = logger;
    }

    public async Task<LoginResponseDTO?> LoginUser(LoginRequestDTO request)
    {
        string username = request.Username;
        string password = request.Password;

        User? user = _database.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            _logger.LogError("User does not exists");
            return null;
        }

        PasswordHelper passHelper = new PasswordHelper();
        bool isPasswordValid = passHelper.VerifyPassword(user, user.PasswordHash, password);
        if (!isPasswordValid)
        {
            _logger.LogError("Invalid Password");
            return null;  
        } 
        string tokenString = GenerateJWT(user);

        _logger.LogInformation("Login Request successful");

        return new LoginResponseDTO
        {
            Token = tokenString, 
            ExpiresIn = 3600,
            User = new UserField
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Role = user.Role
            }
        };
    }

    private string GenerateJWT(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username) ,
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.UtcNow.AddMinutes(480),
            signingCredentials: credentials
        );
        _logger.LogInformation("JWT Generated Successfully");

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
