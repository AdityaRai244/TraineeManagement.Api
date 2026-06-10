namespace TraineeManagement.Api.Models;


public class UserField
{
    public string Id {get; set;} = string.Empty;
    public string Username {get;set;} = string.Empty;
    public UserRole Role{get;set;}

}

public class LoginResponse
{
    public string Token {get; set;} = string.Empty;
    public int ExpiresIn {get; set;}

    public required UserField User {get;set;}
}