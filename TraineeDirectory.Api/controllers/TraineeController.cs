
using httpClient.services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/trainees")]
public class TraineeController : ControllerBase
{

    private readonly ITraineeService traineeService;
    public TraineeController(ITraineeService traineeService)
    {
        this.traineeService = traineeService;

    }

    public ActionResult Get()
    {

        var trainee = traineeService.GetUserDataAsync();
        Console.WriteLine(trainee);

        return Ok(trainee);

    }



}