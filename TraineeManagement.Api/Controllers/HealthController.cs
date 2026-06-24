using System.Collections;
using Microsoft.AspNetCore.Mvc;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/health")]
public class HealthController : ControllerBase
{
    private readonly ILogger<TraineeController> _logger;
    
      public HealthController(ILogger<TraineeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable> HealthCheck()
    {
        _logger.LogInformation("Reached Health API Successfully");
        return Ok(new {status = "running", application = "Trainee Management API", timestamp = DateTime.UtcNow });
    }

}

