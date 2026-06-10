using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/auth/login")]
public class LoginController : ControllerBase
{

    private readonly IAuthService authService;
    public LoginController(IAuthService authService)
    {
        this.authService = authService;
    }
    
    [HttpPost]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        
        var response = await authService.LoginUser(request);
        if(response == null)
        {
            return NotFound(new {message = $"{request.Username} is an Invalid Username or Password is incorrect."});
        }
        return Ok(response);


    }

}