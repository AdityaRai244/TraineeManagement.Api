using System.Collections;
using Microsoft.AspNetCore.Mvc;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/health")]
public class HealthController : ControllerBase
{
    
    [HttpGet]
    public ActionResult<IEnumerable> HealthCheck()
    {
        return Ok(new {status = "running", application = "Trainee Management API", timestamp = DateTime.Now });
    }

}

