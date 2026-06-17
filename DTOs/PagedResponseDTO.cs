using System.Text.Json.Serialization;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.DTOs;


public class PagedResponseDTO
{

    public int PageNumber {get; set;}
    public int PageSize {get; set;}
    public int TotalRecords {get; set;}
    public  List<TraineeResponseDTO> Data {get;set;} = new List<TraineeResponseDTO>();

}
