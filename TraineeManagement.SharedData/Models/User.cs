namespace TraineeManagement.SharedData.Models;
using Microsoft.EntityFrameworkCore;

public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int Id {get; set;}
    public  string Username {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public UserRole Role {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
}