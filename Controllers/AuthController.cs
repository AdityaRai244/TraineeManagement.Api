using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/auth/login")]
public class LoginController : ControllerBase
{

    private readonly IAuthService authService;
    private readonly ILogger<TraineeController> _logger;

    public LoginController(IAuthService authService, ILogger<TraineeController> logger)
    {
        this.authService = authService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
    {
        
        var response = await authService.LoginUser(request);
        if(response == null)
        {
            _logger.LogError("Invalid Username Or Password");
            return NotFound(new {message = $"{request.Username} is an Invalid Username or Password is incorrect."});
        }
        _logger.LogInformation("Logged in successfully");
        return Ok(response);


    }

}