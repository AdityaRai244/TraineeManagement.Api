using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/learning-tasks")]

public class LearningTaskController : ControllerBase
{
    
    private readonly ILearningTaskService _learningTask;
    private readonly ILogger<LearningTaskController> _logger;
    public LearningTaskController(ILearningTaskService learningTask, ILogger<LearningTaskController> logger)
    {
        _learningTask = learningTask;
        _logger = logger;

    }

     [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get( [FromQuery] LearningTaskStatus? status, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
		    return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var tasks = await _learningTask.GetAllTasks(status,search,pageNumber,pageSize);
        _logger.LogInformation("Tasks fetched from service successfully");
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var task = await _learningTask.GetTaskById(id);
        if(task == null)
        {
            _logger.LogError("Task with Id {id} Not found",id);
            return NotFound(new {message = $"Task with {id} not found"});
        }
        _logger.LogInformation("Task with Id {id} Fetched from service Successfully",id);
        return Ok(task);

    }

    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateLearningTaskDTO request)
    {

        var task = await _learningTask.CreateTask(request);
        _logger.LogInformation("Task Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new {id = task.Id}, task);

    }

     [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Put(int id, [FromBody] UpdateLearningTaskDTO request)
    {
        var task = await _learningTask.UpdateTask(id, request);
        if(task == null)
        {
            _logger.LogError("Task with Id {id} Not found",id);
            return NotFound(new {message = $"Task with {id} not found"});
        }
        _logger.LogInformation("Task Updated From Service Successfully. ID : {id}",id);
        return Ok(task);

    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _learningTask.DeleteTask(id);
        if (!success)
        {
            _logger.LogError("Task with Id {id} Not found",id);
            return NotFound(new {message = $"Task with {id} not found"});
        }
        _logger.LogInformation("Task Deleted From Service Successfully. ID : {id}",id);
        return NoContent();
    }

}