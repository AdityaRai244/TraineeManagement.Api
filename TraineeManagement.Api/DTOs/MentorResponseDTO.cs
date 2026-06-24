namespace TraineeManagement.Api.DTOs;

using System.Text.Json.Serialization;
using TraineeManagement.SharedData.Models;


public class MentorResponseDTO
{
    
    public int Id {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Expertise {get; set;} = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MentorStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}

}