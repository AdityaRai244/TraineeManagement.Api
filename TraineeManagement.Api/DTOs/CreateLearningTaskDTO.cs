namespace TraineeManagement.Api.DTOs;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.SharedData.Models;



public class CreateLearningTaskDTO
{
    [Required(ErrorMessage = "Title is Required")]
    public string Title {get; set;} = string.Empty;

    [Required(ErrorMessage = "Description is Required")]
    public string Description {get; set;} = string.Empty;


    [Required(ErrorMessage = "ExpectedTechStack is Required")]
    public string ExpectedTechStack {get; set;} = string.Empty;

    [Required(ErrorMessage = "Due date is required")]
    public DateTime DueDate {get; set;}

    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(LearningTaskStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LearningTaskStatus Status {get; set;}

}