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
    public async Task<ActionResult> Get([FromQuery] string? search)
    {
        return Ok(await traineeService.GetAllTrainees(search));
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {

        var trainee = await traineeService.GetTraineeById(id);
        if(trainee == null)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return Ok(trainee);

    }
    

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateTraineeRequest request)
    {

        var trainee = await traineeService.CreateTrainee(request);
        return CreatedAtAction(nameof(GetById), new {id =trainee.Id}, trainee);

    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] UpdateTraineeRequest request)
    {
        var trainee = await traineeService.UpdateTrainee(id, request);
        if(trainee == null)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return Ok(trainee);

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await traineeService.DeleteTrainee(id);
        if (!success)
        {
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        return NoContent();
    }


}