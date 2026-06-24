namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;


public interface ISubmissionService
{
    
    Task<IEnumerable<SubmissionResponseDTO>> GetAllSubmissions(SubmissionStatus? status,int pageNumber = 1, int pageSize = 10);
    Task<SubmissionResponseDTO?> GetSubmissionById(int id);
    Task<SubmissionResponseDTO> CreateSubmission(CreateSubmissionDTO task);
    Task<SubmissionSummaryDTO> GetSubmissionSummaryById(int submissionId);

}