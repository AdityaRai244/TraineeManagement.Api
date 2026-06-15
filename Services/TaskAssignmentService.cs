using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

public class TaskAssignmentService : ITaskAssignmentService
{
    
    

    private readonly AppDbContext database;
    private readonly ILogger<ReviewService> _logger;

    public TaskAssignmentService(AppDbContext database, ILogger<ReviewService> logger)
    {
        this.database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskAssignmentResponseDTO>> GetAllTaskAssignment(TaskAssignmentStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = database.TaskAssignment.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status.ToString()))
        {
            query = query.Where(t => string.Equals(t.Status.ToString(),status.ToString()));
             _logger.LogInformation("Implemented Status Filtering");
        }

        var TaskAssignment = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
             _logger.LogInformation("Implemented Pagination");

        // var mentors = await query.ToListAsync();
        return TaskAssignment.Select(MapResponse);

    }

    public async Task<TaskAssignmentResponseDTO?> GetTaskAssignmentById(int id)
    {

        var TaskAssignment = await database.TaskAssignment.FindAsync(id);
        _logger.LogInformation("Get Task Assignment By Id Request Successful for Id No : {id}",id);
        return TaskAssignment == null ? null : MapResponse(TaskAssignment);
    }

    public async Task<TaskAssignmentResponseDTO> CreateTaskAssignment(CreateTaskAssignmentDTO request)
    {

        if (request.DueDate < request.AssignedDate)
        {
            throw new ArgumentException("The due date cannot be earlier than the assigned date.");
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

        await database.TaskAssignment.AddAsync(TaskAssignment);

        await database.SaveChangesAsync();
        _logger.LogInformation("TaskAssignment Created Succesfully");
        return MapResponse(TaskAssignment);

    }

    public async Task<TaskAssignmentResponseDTO?> UpdateTaskAssignment(int id, UpdateTaskAssignmentDTO request)
    {

        var TaskAssignment = await database.TaskAssignment.FindAsync(id);

        if (TaskAssignment == null) return null;
        TaskAssignment.Status = request.Status;

        await database.SaveChangesAsync();
        _logger.LogInformation("Update Task Assignment Request Successful for Id No : {id}",id);

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