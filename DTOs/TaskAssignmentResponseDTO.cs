namespace TraineeManagement.Api.DTOs;

using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;


public class TaskAssignmentResponseDTO
{
    
    public int Id {get; set;}

    public int TraineeId {get; set;}
    public int MentorId {get; set;}
    public int LearningTaskId {get; set;}
    public DateTime AssignedDate {get; set;}
    public DateTime DueDate {get; set;}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskAssignmentStatus Status {get; set;}
    public string? Remarks {get; set;}

}