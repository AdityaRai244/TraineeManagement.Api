using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs;

public class UpdateTaskAssignmentDTO
{
    
    
    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskAssignmentStatus Status {get; set;}

}