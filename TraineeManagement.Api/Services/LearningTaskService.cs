using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

public class LearningTaskService : ILearningTaskService
{



    private readonly AppDbContext _database;
    private readonly ILogger<AuthService> _logger;

    public LearningTaskService(AppDbContext database, ILogger<AuthService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<LearningTaskResponseDTO>> GetAllTasks(LearningTaskStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _database.LearningTasks.AsQueryable();
       
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search) ||
                t.ExpectedTechStack.ToLower().Contains(search)
             );

            _logger.LogInformation("Implemented Search Filtering");
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
            _logger.LogInformation("Implemented Status Filtering");
        }


        var mentors = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        _logger.LogInformation("Implemented Pagination");

        // var mentors = await query.ToListAsync();
        return mentors.Select(MapResponse);

    }

    public async Task<LearningTaskResponseDTO?> GetTaskById(int id)
    {

        var task = await _database.LearningTasks.FindAsync(id);
        if (task == null)
        {
            throw new NotFoundException("Task");
        }
        _logger.LogInformation("Get Task By Id Request Successful for Id No : {id}", id);
        return task == null ? null : MapResponse(task);
    }

    public async Task<LearningTaskResponseDTO> CreateTask(CreateLearningTaskDTO request)
    {

        var task = new LearningTask
        {
            Title = request.Title,
            Description = request.Description,
            ExpectedTechStack = request.ExpectedTechStack,
            Status = request.Status,
            DueDate = request.DueDate,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow

        };

        await _database.LearningTasks.AddAsync(task);

        await _database.SaveChangesAsync();
        _logger.LogInformation("Task Created Succesfully");
        return MapResponse(task);

    }

    public async Task<LearningTaskResponseDTO?> UpdateTask(int id, UpdateLearningTaskDTO request)
    {

        var task = await _database.LearningTasks.FindAsync(id);

        if (task == null)
        {
            throw new NotFoundException("Task");
        }
        task.Title = request.Title;
        task.Description = request.Description;
        task.ExpectedTechStack = request.ExpectedTechStack;
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.UpdatedDate = DateTime.UtcNow;

        await _database.SaveChangesAsync();
        _logger.LogInformation("Update task Request Successful for Id No : {id}", id);

        return MapResponse(task);

    }

    public async Task<bool> DeleteTask(int id)
    {
        var task = await _database.LearningTasks.FindAsync(id);
        if (task == null)
        {
            throw new NotFoundException("Task");
        }
        _database.LearningTasks.Remove(task);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Delete Task Successful for Id No : {id}", id);
        return true;
    }
    public LearningTaskResponseDTO MapResponse(LearningTask newTask)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new LearningTaskResponseDTO
        {

            Id = newTask.Id,
            Title = newTask.Title,
            Description = newTask.Description,
            ExpectedTechStack = newTask.ExpectedTechStack,
            DueDate = newTask.DueDate,
            Status = newTask.Status,
            CreatedDate = newTask.CreatedDate,
            UpdatedDate = newTask.UpdatedDate
        };


    }




}