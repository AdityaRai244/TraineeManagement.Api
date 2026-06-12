namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

public interface ITaskAssignmentService
{
    
    Task<IEnumerable<TaskAssignmentResponseDTO>> GetAllTaskAssignment(TaskAssignmentStatus? status, string? search = null,int pageNumber = 1, int pageSize = 10);
    Task<TaskAssignmentResponseDTO?> GetTaskAssignmentById(int id);
    Task<TaskAssignmentResponseDTO> CreateTaskAssignment(CreateTaskAssignmentDTO task);
    Task<TaskAssignmentResponseDTO?> UpdateTaskAssignment(int id, UpdateTaskAssignmentDTO task);

}