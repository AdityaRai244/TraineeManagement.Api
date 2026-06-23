using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

public class SubmissionService : ISubmissionService
{

    private readonly AppDbContext database;
    private readonly ILogger<SubmissionService> _logger;
    private readonly IRedisService<SubmissionSummaryDTO> _redisCache;

    public SubmissionService(IRedisService<SubmissionSummaryDTO> redisCache, AppDbContext database, ILogger<SubmissionService> logger)
    {
        this.database = database;
        _logger = logger;
        _redisCache = redisCache;
    }

    public async Task<IEnumerable<SubmissionResponseDTO>> GetAllSubmissions(SubmissionStatus? status, int pageNumber = 1, int pageSize = 10)
    {
        var query = database.Submission.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
            _logger.LogInformation("Implemented Status Filtering");
        }


        var Submission = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        _logger.LogInformation("Implemented Pagination");

        return Submission.Select(MapResponse);

    }

    public async Task<SubmissionResponseDTO?> GetSubmissionById(int id)
    {

        var Submission = await database.Submission.FindAsync(id);
        if (Submission == null)
        {
            throw new NotFoundException("Submission");
        }
        _logger.LogInformation("Get Submission By Id Request Successful for Id No : {id}", id);
        return MapResponse(Submission);
    }

    public async Task<SubmissionResponseDTO> CreateSubmission(CreateSubmissionDTO request)
    {

        var taskAssignmentExists = await database.TaskAssignment.FirstOrDefaultAsync(t => t.Id == request.TaskAssignmentId);
        if (taskAssignmentExists == null)
        {
            throw new NotFoundException("TaskAssignment");
        }

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

    public async Task<SubmissionSummaryDTO> GetSubmissionSummaryById(int submissionId)
    {
        string idString = submissionId.ToString();
        var summary = await _redisCache.GetAsync(idString);

        if (summary is not null)
        {
            _logger.LogInformation("Cache HIT for trainee {Id}", submissionId);
            return summary;
        }
        Submission Submission = await database.Submission.FindAsync(submissionId);
        if (Submission == null)
        {
            throw new NotFoundException("Submission");
        }
        _logger.LogInformation("Get Submission By Id Request Successful for Id No : {id}", submissionId);
        return new SubmissionSummaryDTO
        {
            TaskAssignmentId = Submission.TaskAssignmentId,
            Status = Submission.Status,
            SubmissionUrl = Submission.SubmissionUrl,
            SubmittedDate= Submission.SubmittedDate
        };

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