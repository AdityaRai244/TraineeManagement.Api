using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface IAuthService
{
    
    Task<LoginResponse?> LoginUser(LoginRequest request);


}