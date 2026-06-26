
using httpClient.services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api")]
public class TraineeController : ControllerBase
{

    private readonly ITraineeService _traineeService;
    private readonly ILogger<TraineeController> _logger;

    public TraineeController(ITraineeService traineeService, ILogger<TraineeController> logger)
    {
        _traineeService = traineeService;
        _logger = logger;

    }

     [HttpGet("health")]
    public IActionResult  Get()
    {

       return Ok("Responding from TraineeDirectory.Api");
    }

    [HttpGet("trainees/{id}/{correlationId}")]
    public ActionResult Get(int id, Guid correlationId)
    {

        _logger.LogInformation($"Correlation id received {correlationId}");
        var trainee = _traineeService.GetUserDataAsync(id);
        _logger.LogInformation($"Trainee retreived from TraineeDirectory.Api {trainee}");

        return Ok(trainee);

    }



}