namespace TraineeManagement.Api;

public class Trainee
{
    public int Id {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string TechStack {get; set;} = string.Empty;
    public string Status {get; set;} = string.Empty;
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

}