using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ITraineeService
{
    
    Task<IEnumerable<TraineeResponse>> GetAllTrainees(UserStatus? status, string? search = null,int pageNumber = 1, int pageSize = 10);
    Task<TraineeResponse?> GetTraineeById(int id);
    Task<TraineeResponse> CreateTrainee(CreateTraineeRequest trainee);
    Task<TraineeResponse?> UpdateTrainee(int id, UpdateTraineeRequest trainee);
    Task<bool> DeleteTrainee(int id);



}