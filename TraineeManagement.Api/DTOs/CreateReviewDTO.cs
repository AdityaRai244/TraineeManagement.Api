using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TraineeManagement.SharedData.Models;


namespace TraineeManagement.Api.DTOs;

public class CreateReviewDTO
{
    
    
    [Required(ErrorMessage = "Submission is Required")]
    public int SubmissionId {get; set;}

    [Required(ErrorMessage = "Mentor is Required")]
    public int MentorId {get; set;}

    [Required(ErrorMessage = "Feedback is Required")]
    public string Feedback {get; set;} = string.Empty;

    [Required(ErrorMessage = "Score is Required")]
    public int? Score {get; set;}
    
    [Required(ErrorMessage = "Status is Required")]
    [EnumDataType(typeof(ReviewStatus), ErrorMessage = "Status is Invalid")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReviewStatus ReviewStatus {get; set;}

    [Required(ErrorMessage = "Reviewed Date is Required")]
    public DateTime ReviewedDate {get; set;}


}