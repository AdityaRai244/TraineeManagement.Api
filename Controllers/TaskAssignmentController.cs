using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/task-assignments")]

public class TaskAssignmentController : ControllerBase
{

    private readonly ITaskAssignmentService taskAssignmentService;
    private readonly ILogger<TaskAssignmentController> _logger;
    public TaskAssignmentController(ITaskAssignmentService learningTask, ILogger<TaskAssignmentController> logger)
    {
        this.taskAssignmentService = learningTask;
        _logger = logger;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] TaskAssignmentStatus? status, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var taskAssignments = await taskAssignmentService.GetAllTaskAssignment(status, search, pageNumber, pageSize);
        _logger.LogInformation("Task Assignments fetched from service successfully");
        return Ok(taskAssignments);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var taskAssignment = await taskAssignmentService.GetTaskAssignmentById(id);
        if (taskAssignment == null)
        {
            _logger.LogError("Task Assignment with Id {id} Not found", id);
            return NotFound(new { message = $"Task Assignment with {id} not found" });
        }
        _logger.LogInformation("Task Assignment with Id {id} Fetched from service Successfully", id);
        return Ok(taskAssignment);

    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateTaskAssignmentDTO request)
    {

        var taskAssignment = await taskAssignmentService.CreateTaskAssignment(request);
        _logger.LogInformation("Task Assignment Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new { id = taskAssignment.Id }, taskAssignment);

    }

    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<ActionResult> Put(int id, [FromBody] UpdateTaskAssignmentDTO request)
    {
        var taskAssignment = await taskAssignmentService.UpdateTaskAssignment(id, request);
        if (taskAssignment == null)
        {
            _logger.LogError("Task Assignment with Id {id} Not found", id);
            return NotFound(new { message = $"Task Assignment with {id} not found" });
        }
        _logger.LogInformation("Task Assignment Updated From Service Successfully. ID : {id}", id);
        return Ok(taskAssignment);

    }



}