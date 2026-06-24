namespace TraineeManagement.SharedData.Models;


public enum TaskAssignmentStatus
{
    Assigned,
    InProgress,
    Submitted,
    Reviewed,
    Completed
}

public class TaskAssignment
{
    
    public int Id {get; set;}

    public int TraineeId {get; set;}
    public int MentorId {get; set;}
    public int LearningTaskId {get; set;}
    public DateTime AssignedDate {get; set;}
    public DateTime DueDate {get; set;}

    public TaskAssignmentStatus Status {get; set;}
    public string? Remarks {get; set;}

    public Trainee Trainee { get; set; } = null!;
    public Mentor Mentor { get; set; } = null!;
    public LearningTask LearningTask { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();



}