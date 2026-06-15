using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs;

public class UpdateTraineeRequestDTO
{
    
    [Required(ErrorMessage = "First Name is Required")]
    [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    public string FirstName {get; set;} = string.Empty;

    [Required(ErrorMessage = "Last Name is Required")]
    [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    public string LastName {get; set;} = string.Empty;

    [Required(ErrorMessage = "Email Id is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Format")]
    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "Tech Stack is Required")]
    public string TechStack {get; set;} = string.Empty;


    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(UserStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status {get;set;}

}