using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs;

public class CreateSubmissionDTO
{
    
    
    [Required(ErrorMessage = "Task Assignment is Required")]
    public int TaskAssignmentId {get; set;}

    [Required(ErrorMessage = "Submission URL is Required")]
    [EnumDataType(typeof(SubmissionOptions), ErrorMessage = "Submission URL is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubmissionOptions SubmissionUrl {get; set;}

    
    [Required(ErrorMessage = "Notes is Required")]
    public string Notes {get; set;} = string.Empty;

    [Required(ErrorMessage = "Submitted Date is Required")]
    public DateTime SubmittedDate {get; set;}

    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(SubmissionStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubmissionStatus Status {get; set;}

}