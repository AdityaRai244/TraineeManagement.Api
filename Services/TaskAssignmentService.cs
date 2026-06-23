using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

public class TaskAssignmentService : ITaskAssignmentService
{



    private readonly AppDbContext _database;
    private readonly ILogger<TaskAssignmentService> _logger;
    private readonly IRedisService<TaskAssignmentResponseDTO> _redisCache;

    public TaskAssignmentService(IRedisService<TaskAssignmentResponseDTO> redisCache, AppDbContext database, ILogger<TaskAssignmentService> logger)
    {
        _redisCache = redisCache;
        _database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskAssignmentResponseDTO>> GetAllTaskAssignment(TaskAssignmentStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _database.TaskAssignment.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
            _logger.LogInformation("Implemented Status Filtering");
        }


        var TaskAssignment = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        _logger.LogInformation("Implemented Pagination");

        // var mentors = await query.ToListAsync();
        return TaskAssignment.Select(MapResponse);

    }

    public async Task<TaskAssignmentResponseDTO?> GetTaskAssignmentById(int id)
    {

        string idString = id.ToString();
        var taskById = await _redisCache.GetAsync(idString);

        if (taskById is not null)
        {
            _logger.LogInformation("Cache HIT for taskById {Id}", id);
            return taskById;
        }

        _logger.LogWarning("Cache MISS for Review {Id}. Accessing DB.", id);


        var TaskAssignmentDB = await _database.TaskAssignment.FindAsync(id);
        if (TaskAssignmentDB == null)
        {
            throw new NotFoundException("TaskAssignment");
        }

        TaskAssignmentResponseDTO mappedResponse = MapResponse(TaskAssignmentDB);
        await _redisCache.SetAsync(idString, mappedResponse);
        _logger.LogInformation("Cache updated for Task Assignment {Id}", id);
        _logger.LogInformation("Get Task By Id Request Successful for Id No : {id}", id);

        return mappedResponse;
    }

    public async Task<TaskAssignmentResponseDTO> CreateTaskAssignment(CreateTaskAssignmentDTO request)
    {

        var traineeExists = await _database.Trainees.FirstOrDefaultAsync(t => t.Id == request.TraineeId);
        if (traineeExists == null)
        {
            throw new NotFoundException("Trainee");
        }

        var mentorExists = await _database.Mentors.FirstOrDefaultAsync(t => t.Id == request.MentorId);
        if (mentorExists == null)
        {
            throw new NotFoundException("Mentor");
        }


        var learningTasks = await _database.LearningTasks.FirstOrDefaultAsync(t => t.Id == request.LearningTaskId);
        if (learningTasks == null)
        {
            throw new NotFoundException("Learning Task");
        }



        if (request.DueDate < request.AssignedDate)
        {
            throw new BadRequestException("The due date cannot be earlier than the assigned date.");
        }

        var TaskAssignment = new TaskAssignment
        {
            TraineeId = request.TraineeId,
            MentorId = request.MentorId,
            LearningTaskId = request.LearningTaskId,
            AssignedDate = request.AssignedDate,
            DueDate = request.DueDate,
            Status = request.Status,
            Remarks = request.Remarks

        };

        await _database.TaskAssignment.AddAsync(TaskAssignment);

        await _database.SaveChangesAsync();
        _logger.LogInformation("TaskAssignment Created Succesfully");
        return MapResponse(TaskAssignment);

    }

    public async Task<TaskAssignmentResponseDTO?> UpdateTaskAssignment(int id, UpdateTaskAssignmentDTO request)
    {

        var TaskAssignment = await _database.TaskAssignment.FindAsync(id);

        if (TaskAssignment == null)
        {
            throw new NotFoundException("TaskAssignment");
        }
        TaskAssignment.Status = request.Status;

        await _database.SaveChangesAsync();
        _logger.LogInformation("Update Task Assignment Request Successful for Id No : {id}", id);

        await _redisCache.RemoveAsync(id.ToString());

        _logger.LogInformation("Cached Updated for Task Assignment Id No : {id}", id);


        return MapResponse(TaskAssignment);

    }


    public TaskAssignmentResponseDTO MapResponse(TaskAssignment newTaskAssignment)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new TaskAssignmentResponseDTO
        {
            Id = newTaskAssignment.Id,
            TraineeId = newTaskAssignment.TraineeId,
            MentorId = newTaskAssignment.MentorId,
            LearningTaskId = newTaskAssignment.LearningTaskId,
            AssignedDate = newTaskAssignment.AssignedDate,
            DueDate = newTaskAssignment.DueDate,
            Status = newTaskAssignment.Status,
            Remarks = newTaskAssignment.Remarks
        };


    }




}