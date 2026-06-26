using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api")]

public class SubmissionController : ControllerBase
{

    private readonly ISubmissionService _submissionService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IConfiguration _config;
    private readonly AppDbContext _database;

    private readonly ILogger<SubmissionController> _logger;
    public SubmissionController(ISubmissionService submissionService, AppDbContext database, IFileStorageService fileStorageService, ILogger<SubmissionController> logger, IConfiguration config)
    {
        _submissionService = submissionService;
        _database = database;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _config = config;

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

        var submissions = await _submissionService.GetAllSubmissions(status, pageNumber, pageSize);
        _logger.LogInformation("Submissions fetched from service successfully");
        return Ok(submissions);
    }

    [HttpGet("{submissionId}/summary")]
    [Authorize]
    public async Task<ActionResult> GetSummary([FromRoute] int submissionId)
    {

        var summary = await _submissionService.GetSubmissionSummaryById(submissionId);
        _logger.LogInformation("Summary fetched from service successfully");
        return Ok(summary);
    }

    [HttpGet("submissions/{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var submission = await _submissionService.GetSubmissionById(id);
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

        var submission = await _submissionService.CreateSubmission(request);
        _logger.LogInformation("Submission Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, submission);

    }

    [HttpPost]
    [Authorize]
    [Route("submissions/{submissionId}/files")]
    public async Task<IActionResult> UploadFile(int submissionId, IFormFile file)
    {
      
        var result = await _fileStorageService.SaveAsync(submissionId, file);
        Console.WriteLine(result);
        return Accepted(result);
    }


    [HttpGet]
    [Authorize]
    [Route("submission-files/{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {

        var metaData = await _database.SubmissionFile.FindAsync(id);
        if (metaData == null)
        {
            throw new NotFoundException("File does not exists");
        }
        var stream = await _fileStorageService.OpenReadAsync(metaData.StorageName);
        return File(stream, metaData.ContentType, metaData.OriginalFileName);


    }

    [HttpDelete]
    [Authorize]
    [Route("submission-files/{id}/delete")]
    public async Task<IActionResult> DeleteFile(int id)
    {

        var metaData = await _database.SubmissionFile.FindAsync(id);
        if (metaData == null)
        {
            throw new NotFoundException("File does not exists");
        }
        await _fileStorageService.DeleteAsync(metaData.StorageName);
        _database.SubmissionFile.Remove(metaData);
        await _database.SaveChangesAsync();
        return NoContent();


    }

}