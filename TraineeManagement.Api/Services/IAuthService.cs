using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;


namespace TraineeManagement.Api.Services;

public interface IAuthService
{
    
    Task<LoginResponseDTO?> LoginUser(LoginRequestDTO request);


}