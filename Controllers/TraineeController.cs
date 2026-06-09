using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/trainees")]
public class TraineeController : ControllerBase
{

    private readonly ITraineeService traineeService;
    public TraineeController(ITraineeService traineeService)
    {
        this.traineeService = traineeService;
    }


    [HttpGet]
    public ActionResult Get()
    {
        return Ok(traineeService.GetAllTrainees());
    }

    [HttpGet("{id}")]
    public ActionResult GetById(int id)
    {

        var trainee = traineeService.GetTraineeById(id);
        if(trainee == null)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return Ok(trainee);

    }
    

    [HttpPost]
    public ActionResult Post([FromBody] CreateTraineeRequest request)
    {

        var trainee = traineeService.CreateTrainee(request);
        return Ok(trainee);

    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] UpdateTraineeRequest request)
    {
        var trainee = traineeService.UpdateTrainee(id, request);
        if(trainee == null)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return Ok(trainee);

    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var success = traineeService.DeleteTrainee(id);
        if (!success)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return NoContent();
    }

}