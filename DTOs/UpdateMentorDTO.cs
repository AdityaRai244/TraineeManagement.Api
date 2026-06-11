namespace TraineeManagement.Api.DTOs;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;


public class UpdateMentorDTO
{
    [Required(ErrorMessage = "First Name is Required")]
    public string FirstName {get; set;} = string.Empty;

    [Required(ErrorMessage = "Last Name is Required")]
    public string LastName {get; set;} = string.Empty;

    [Required(ErrorMessage = "Email Id is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Format")]
    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "Expertise is Required")]
    public string Expertise {get; set;} = string.Empty;

    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(MentorStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MentorStatus Status {get; set;}

}