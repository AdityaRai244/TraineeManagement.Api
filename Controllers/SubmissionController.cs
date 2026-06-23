using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api")]

public class SubmissionController : ControllerBase
{

    private readonly ISubmissionService submissionService;
    private readonly IFileStorageService fileStorageService;
    private readonly IConfiguration config;
    private readonly AppDbContext database;

    private readonly ILogger<SubmissionController> logger;
    public SubmissionController(ISubmissionService submissionService, AppDbContext database, IFileStorageService fileStorageService, ILogger<SubmissionController> logger, IConfiguration config)
    {
        this.submissionService = submissionService;
        this.database = database;
        this.fileStorageService = fileStorageService;
        this.logger = logger;
        this.config = config;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] SubmissionStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            logger.LogError("Invalid Paramters");
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var submissions = await submissionService.GetAllSubmissions(status, pageNumber, pageSize);
        logger.LogInformation("Submissions fetched from service successfully");
        return Ok(submissions);
    }

    [HttpGet("{submissionId}/summary")]
    [Authorize]
    public async Task<ActionResult> GetSummary([FromRoute] int submissionId)
    {

        var summary = await submissionService.GetSubmissionSummaryById(submissionId);
        logger.LogInformation("Summary fetched from service successfully");
        return Ok(summary);
    }

    [HttpGet("submissions/{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var submission = await submissionService.GetSubmissionById(id);
        if (submission == null)
        {
            logger.LogError("Submission with Id {id} Not found", id);
            return NotFound(new { message = $"Submission with {id} not found" });
        }
        logger.LogInformation("Submission with Id {id} Fetched from service Successfully", id);
        return Ok(submission);

    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateSubmissionDTO request)
    {

        var submission = await submissionService.CreateSubmission(request);
        logger.LogInformation("Submission Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, submission);

    }

    [HttpPost]
    [Authorize]
    [Route("submissions/{submissionId}/files")]
    public async Task<IActionResult> UploadFile(int submissionId, IFormFile file)
    {

        var result = await fileStorageService.SaveAsync(submissionId, file);

        return Accepted(result);
    }


    [HttpGet]
    [Authorize]
    [Route("submission-files/{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {

        var metaData = await database.SubmissionFile.FindAsync(id);
        if (metaData == null)
        {
            throw new NotFoundException("File does not exists");
        }
        var stream = await fileStorageService.OpenReadAsync(metaData.StorageName);
        return File(stream, metaData.ContentType, metaData.OriginalFileName);


    }

    [HttpDelete]
    [Authorize]
    [Route("submission-files/{id}/delete")]
    public async Task<IActionResult> DeleteFile(int id)
    {

        var metaData = await database.SubmissionFile.FindAsync(id);
        if (metaData == null)
        {
            throw new NotFoundException("File does not exists");
        }
        await fileStorageService.DeleteAsync(metaData.StorageName);
        database.SubmissionFile.Remove(metaData);
        await database.SaveChangesAsync();
        return NoContent();


    }

}