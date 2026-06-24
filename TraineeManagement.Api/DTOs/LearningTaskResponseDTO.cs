using System.Text.Json.Serialization;
using TraineeManagement.SharedData.Models;

namespace TraineeManagement.Api.DTOs;



public class LearningTaskResponseDTO
{
    
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string ExpectedTechStack {get; set;} = string.Empty;
    public DateTime DueDate {get; set;}
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LearningTaskStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

}