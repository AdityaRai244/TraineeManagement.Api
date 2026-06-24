using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/processing-jobs")]

public class ProcessingJobController : ControllerBase
{

    private readonly AppDbContext _database;
    private readonly ILogger<ReviewController> _logger;
    public ProcessingJobController(AppDbContext database, ILogger<ReviewController> logger)
    {
        _logger = logger;
        _database = database;
    }

    [HttpGet("/{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var job = await _database.ProcessingJob.FirstOrDefaultAsync(t => t.Id == id);
        if(job == null)
        {
        _logger.LogError("Job with Id {id} does not exists", id);
        }
        _logger.LogInformation("Job with Id {id} Fetched from service Successfully", id);
        return Ok(job);

    }

}