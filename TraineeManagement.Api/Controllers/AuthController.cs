using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[EnableRateLimiting("fixed")]
[Route("/api/auth/login")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;
    private readonly ILogger<TraineeController> _logger;

    public AuthController(IAuthService authService, ILogger<TraineeController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
    {
        
        var response = await _authService.LoginUser(request);
        if(response == null)
        {
            _logger.LogError("Invalid Username Or Password");
            return NotFound(new {message = $"{request.Username} is an Invalid Username or Password is incorrect."});
        }
        _logger.LogInformation("Logged in successfully");
        return Ok(response);


    }

}