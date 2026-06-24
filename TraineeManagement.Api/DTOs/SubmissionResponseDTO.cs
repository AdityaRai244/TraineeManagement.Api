using TraineeManagement.SharedData.Models;


namespace TraineeManagement.Api.DTOs;


public class SubmissionResponseDTO
{
    
    public int Id {get; set;}

    public int TaskAssignmentId {get; set;}
    public required string SubmissionUrl {get; set;}
    public string Notes {get; set;} = string.Empty;
    public DateTime SubmittedDate {get; set;}

    public SubmissionStatus Status {get; set;}
    public TaskAssignment TaskAssignment { get; set; } = null!;

}