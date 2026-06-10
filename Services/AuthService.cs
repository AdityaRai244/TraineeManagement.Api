using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Helpers;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

public class AuthService : IAuthService
{

    private readonly AppDbContext database;
    public AuthService(AppDbContext database){
        this.database = database;
    }
    
    public async Task<LoginResponse?> LoginUser([FromBody] LoginRequest request)
    {

        string username = request.Username;
        string password = request.Password;

        User user = database.Users.SingleOrDefault(u => u.Username == username);

        if(user == null)
        {
            return null;
        }

        PasswordHelper passHelper = new PasswordHelper();
        bool isPasswordValid = passHelper.VerifyPassword(user,user.PasswordHash, password);
        if(!isPasswordValid) return null;


        return new LoginResponse {
            Token = "jwt-token",
            ExpiresIn = 3600,
            User =
           new UserField {
                Id = "asdf",
                Username = "Asdfsadf",
                Role = UserRole.Trainee
            }
        };


    }

}