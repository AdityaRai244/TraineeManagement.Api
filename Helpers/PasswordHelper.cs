using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Helpers;


public class PasswordHelper
{   
    
    public bool VerifyPassword(User user, string hashedPassword, string password)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user,hashedPassword,password);

        return result == PasswordVerificationResult.Success;
    }

}