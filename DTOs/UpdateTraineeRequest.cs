using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Api.DTOs;

public class UpdateTraineeRequest
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
    public string Status {get;set;} = string.Empty;

}