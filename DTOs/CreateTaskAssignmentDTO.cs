using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs;

public class CreateTaskAssignmentDTO
{
    
    
    [Required(ErrorMessage = "Trainee is Required")]
    public int TraineeId {get; set;}

    [Required(ErrorMessage = "Mentor is Required")]
    public int MentorId {get; set;}

    [Required(ErrorMessage = "Learning Task is Required")]
    public int LearningTaskId {get; set;}

    [Required(ErrorMessage = "Assigned Date is Required")]
    public DateTime AssignedDate {get; set;}

    [Required(ErrorMessage = "Due Date is Required")]
    public DateTime DueDate {get; set;}

    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskAssignmentStatus Status {get; set;}
    public string? Remarks {get; set;}

}