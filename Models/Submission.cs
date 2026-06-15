namespace TraineeManagement.Api.Models;

public enum SubmissionOptions
{
    Github,
    Drive,
    Link
}
public enum SubmissionStatus
{
    Submitted,
    Resubmitted
}

public class Submission
{
    
    public int Id {get; set;}

    public int TaskAssignmentId {get; set;}
    public SubmissionOptions SubmissionUrl {get; set;}
    public string Notes {get; set;} = string.Empty;
    public DateTime SubmittedDate {get; set;}

    public SubmissionStatus Status {get; set;}
    public TaskAssignment TaskAssignment { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();


}