using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/submissions")]

public class SubmissionController : ControllerBase
{

    private readonly ISubmissionService submissionService;
    private readonly ILogger<SubmissionController> _logger;
    public SubmissionController(ISubmissionService submissionService, ILogger<SubmissionController> logger)
    {
        this.submissionService = submissionService;
        _logger = logger;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] SubmissionStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var submissions = await submissionService.GetAllSubmissions(status, pageNumber, pageSize);
        _logger.LogInformation("Submissions fetched from service successfully");
        return Ok(submissions);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var submission = await submissionService.GetSubmissionById(id);
        if (submission == null)
        {
            _logger.LogError("Submission with Id {id} Not found", id);
            return NotFound(new { message = $"Submission with {id} not found" });
        }
        _logger.LogInformation("Submission with Id {id} Fetched from service Successfully", id);
        return Ok(submission);

    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateSubmissionDTO request)
    {

        var submission = await submissionService.CreateSubmission(request);
        _logger.LogInformation("Submission Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, submission);

    }

}