using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ITraineeService
{
    
    Task<PagedResponseDTO> GetAllTrainees(UserStatus? status, string? search = null,int pageNumber = 1, int pageSize = 10);
    Task<TraineeResponseDTO?> GetTraineeById(int id);
    Task<TraineeResponseDTO> CreateTrainee(CreateTraineeRequestDTO trainee);
    Task<TraineeResponseDTO?> UpdateTrainee(int id, UpdateTraineeRequestDTO trainee);
    Task<bool> DeleteTrainee(int id);



}