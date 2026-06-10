namespace TraineeManagement.Api.Models;

public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}

public class User
{
    public int Id {get; set;}
    public string Username {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public UserRole Role {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
}