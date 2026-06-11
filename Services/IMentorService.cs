namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

public interface IMentorService
{
    
    Task<IEnumerable<MentorResponseDTO>> GetAllMentors(MentorStatus? status, string? search = null,int pageNumber = 1, int pageSize = 10);
    Task<MentorResponseDTO?> GetMentorById(int id);
    Task<MentorResponseDTO> CreateMentor(CreateMentorDTO trainee);
    Task<MentorResponseDTO?> UpdateMentor(int id, UpdateMentorDTO trainee);
    Task<bool> DeleteMentor(int id);




}