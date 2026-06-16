using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

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

        var TaskAssignment = await database.TaskAssignment.FindAsync(id);
        if (TaskAssignment == null)
        {
            throw new NotFoundException("TaskAssignment");
        }
        _logger.LogInformation("Get Task Assignment By Id Request Successful for Id No : {id}", id);
        return MapResponse(TaskAssignment);
    }

    public async Task<TaskAssignmentResponseDTO> CreateTaskAssignment(CreateTaskAssignmentDTO request)
    {

        var traineeExists = await database.Trainees.FirstOrDefaultAsync(t => t.Id == request.TraineeId);
        if (traineeExists == null)
        {
            throw new NotFoundException("Trainee");
        }

        var mentorExists = await database.Mentors.FirstOrDefaultAsync(t => t.Id == request.MentorId);
        if (mentorExists == null)
        {
            throw new NotFoundException("Mentor");
        }


        var learningTasks = await database.LearningTasks.FirstOrDefaultAsync(t => t.Id == request.LearningTaskId);
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

        await database.TaskAssignment.AddAsync(TaskAssignment);

        await database.SaveChangesAsync();
        _logger.LogInformation("TaskAssignment Created Succesfully");
        return MapResponse(TaskAssignment);

    }

    public async Task<TaskAssignmentResponseDTO?> UpdateTaskAssignment(int id, UpdateTaskAssignmentDTO request)
    {

        var TaskAssignment = await database.TaskAssignment.FindAsync(id);

        if (TaskAssignment == null)
        {
            throw new NotFoundException("TaskAssignment");
        }
        TaskAssignment.Status = request.Status;

        await database.SaveChangesAsync();
        _logger.LogInformation("Update Task Assignment Request Successful for Id No : {id}", id);

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