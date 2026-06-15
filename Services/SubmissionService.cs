using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

public class SubmissionService : ISubmissionService
{

    private readonly AppDbContext database;
    private readonly ILogger<ReviewService> _logger;

    public SubmissionService(AppDbContext database, ILogger<ReviewService> logger)
    {
        this.database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<SubmissionResponseDTO>> GetAllSubmissions(SubmissionStatus? status,  int pageNumber = 1, int pageSize = 10)
    {
        var query = database.Submission.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status.ToString()))
        {
            query = query.Where(t => string.Equals(t.Status.ToString(),status.ToString()));
             _logger.LogInformation("Implemented Status Filtering");
        }

        var Submission = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
             _logger.LogInformation("Implemented Pagination");

        return Submission.Select(MapResponse);

    }

    public async Task<SubmissionResponseDTO?> GetSubmissionById(int id)
    {

        var Submission = await database.Submission.FindAsync(id);
        _logger.LogInformation("Get Submission By Id Request Successful for Id No : {id}",id);
        return Submission == null ? null : MapResponse(Submission);
    }

    public async Task<SubmissionResponseDTO> CreateSubmission(CreateSubmissionDTO request)
    {


        var submission = new Submission
        {
            TaskAssignmentId = request.TaskAssignmentId,
            SubmissionUrl = request.SubmissionUrl,
            Notes = request.Notes,
            SubmittedDate = request.SubmittedDate,
            Status = request.Status,

        };

        await database.Submission.AddAsync(submission);

        await database.SaveChangesAsync();
        _logger.LogInformation("TaskAssignment Created Succesfully");
        return MapResponse(submission);

    }

    public SubmissionResponseDTO MapResponse(Submission newSubmission)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new SubmissionResponseDTO
        {
            Id = newSubmission.Id,
            TaskAssignmentId = newSubmission.TaskAssignmentId,
            SubmissionUrl = newSubmission.SubmissionUrl,
            Notes = newSubmission.Notes,
            SubmittedDate = newSubmission.SubmittedDate,
            Status = newSubmission.Status,
        };


    }




}