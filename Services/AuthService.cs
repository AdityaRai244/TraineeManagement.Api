
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Helpers;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext database;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext database, IConfiguration config)
    {
        this.database = database;
        this._config = config;
    }

    public async Task<LoginResponse?> LoginUser([FromBody] LoginRequest request)
    {
        string username = request.Username;
        string password = request.Password;

        User? user = database.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            return null;
        }

        PasswordHelper passHelper = new PasswordHelper();
        bool isPasswordValid = passHelper.VerifyPassword(user, user.PasswordHash, password);
        if (!isPasswordValid) return null;

        string tokenString = GenerateJWT(user);
        Console.WriteLine(tokenString);

        return new LoginResponse
        {
            Token = tokenString, 
            ExpiresIn = 3600,
            User = new UserField
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Role = UserRole.Trainee
            }
        };
    }

    private string GenerateJWT(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
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
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
