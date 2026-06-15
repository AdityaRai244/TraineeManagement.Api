using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface IAuthService
{
    
    Task<LoginResponseDTO?> LoginUser(LoginRequestDTO request);


}