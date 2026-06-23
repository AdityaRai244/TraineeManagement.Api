using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs;


public class SubmissionSummaryDTO
{
    
    public int Id {get; set;}

    public int TaskAssignmentId {get; set;}
    public required string SubmissionUrl {get; set;}
    public DateTime SubmittedDate {get; set;}

    public SubmissionStatus Status {get; set;}

}