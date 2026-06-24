namespace TraineeManagement.SharedData.Models;


public enum LearningTaskStatus
{
    Draft,
    Published,
    Closed
}

public class LearningTask
{
    
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string ExpectedTechStack {get; set;} = string.Empty;
    public DateTime DueDate {get; set;}

    public LearningTaskStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();


}