using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services;

public interface ITraineeService
{
    
    Task<IEnumerable<TraineeResponse>> GetAllTrainees(string? search = null);
    Task<TraineeResponse?> GetTraineeById(int id);
    Task<TraineeResponse> CreateTrainee(CreateTraineeRequest trainee);
    Task<TraineeResponse?> UpdateTrainee(int id, UpdateTraineeRequest trainee);
    Task<bool> DeleteTrainee(int id);



}