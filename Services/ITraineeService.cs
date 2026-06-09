using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services;

public interface ITraineeService
{
    
    IEnumerable<TraineeResponse> GetAllTrainees();
    TraineeResponse? GetTraineeById(int id);
    TraineeResponse CreateTrainee(CreateTraineeRequest trainee);
    TraineeResponse? UpdateTrainee(int id, UpdateTraineeRequest trainee);
    bool DeleteTrainee(int id);


}