using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/trainees")]
public class TraineeController : ControllerBase
{

    private readonly ITraineeService _traineeService;
    private readonly ILogger<TraineeController> _logger;
    public TraineeController(ITraineeService traineeService, ILogger<TraineeController> logger)
    {
        _traineeService = traineeService;
        _logger = logger;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get( [FromQuery] UserStatus? status, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
		    return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var trainees = await _traineeService.GetAllTrainees(status,search,pageNumber,pageSize);
        _logger.LogInformation("Trainees fetched from service successfully");
        // return Ok(new
        // {
        //     pageNumber = pageNumber,
        //     pageSize = pageSize,
        //     totalRecords = await ,
        //     data = trainees;
        // });
        return Ok(trainees);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var trainee = await _traineeService.GetTraineeById(id);
        if(trainee == null)
        {
            _logger.LogError("Trainee with Id {id} Not found",id);
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        _logger.LogInformation("Trainee with Id {id} Fetched from service Successfully",id);
        return Ok(trainee);

    }
    

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateTraineeRequestDTO request)
    {

        var trainee = await _traineeService.CreateTrainee(request);
        _logger.LogInformation("Trainee Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new {id =trainee.Id}, trainee);

    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Put(int id, [FromBody] UpdateTraineeRequestDTO request)
    {
        var trainee = await _traineeService.UpdateTrainee(id, request);
        if(trainee == null)
        {
            _logger.LogError("Trainee with Id {id} Not found",id);
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        _logger.LogInformation("Trainee Updated From Service Successfully. ID : {id}",id);
        return Ok(trainee);

    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _traineeService.DeleteTrainee(id);
        if (!success)
        {
            _logger.LogError("Trainee with Id {id} Not found",id);
            return NotFound(new {message = $"Trainee with {id} not found"});
        }
        _logger.LogInformation("Trainee Deleted From Service Successfully. ID : {id}",id);
        return NoContent();
    }


}