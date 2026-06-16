using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.DTOs;


public class TraineeResponseDTO
{

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }


}


// using System.Text.Json.Serialization;
// using TraineeManagement.Api.Models;
// namespace TraineeManagement.Api.DTOs;

// public class TraineeData
// {
//      public int Id { get; set; }
//     public string FirstName { get; set; } = string.Empty;
//     public string LastName { get; set; } = string.Empty;
//     public string Email { get; set; } = string.Empty;
//     public string TechStack { get; set; } = string.Empty;

//     [JsonConverter(typeof(JsonStringEnumConverter))]
//     public UserStatus Status { get; set; }
//     public DateTime CreatedDate { get; set; }
//     public DateTime UpdatedDate { get; set; }


// }

// public class TraineeResponseDTO
// {

//     public int PageNumber {get; set;}
//     public int PageSize {get; set;}
//     public int TotalRecords {get; set;}
//     public List<TraineeData> data = new List<TraineeData>;
   
// }