namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

public interface ILearningTaskService
{
    
    Task<IEnumerable<LearningTaskResponseDTO>> GetAllTasks(LearningTaskStatus? status, string? search = null,int pageNumber = 1, int pageSize = 10);
    Task<LearningTaskResponseDTO?> GetTaskById(int id);
    Task<LearningTaskResponseDTO> CreateTask(CreateLearningTaskDTO task);
    Task<LearningTaskResponseDTO?> UpdateTask(int id, UpdateLearningTaskDTO task);
    Task<bool> DeleteTask(int id);


}