namespace TraineeManagement.SharedData.Models;

public enum UserStatus
{
    Active,
    InActive,
    Completed
}

public class Trainee
{

    public int Id {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string TechStack {get; set;} = string.Empty;
    public UserStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

}