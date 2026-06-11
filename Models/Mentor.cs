namespace TraineeManagement.Api.Models;


public enum MentorStatus
{
    Active,
    InActive
}

public class Mentor
{
    
    public int Id {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Expertise {get; set;} = string.Empty;
    public MentorStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

}