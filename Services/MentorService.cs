using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

public class MentorService : IMentorService
{
    
    

    private readonly AppDbContext database;
    private readonly ILogger<AuthService> _logger;

    public MentorService(AppDbContext database, ILogger<AuthService> logger)
    {
        this.database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<MentorResponseDTO>> GetAllMentors(MentorStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = database.Mentors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(t =>
                t.FirstName.ToLower().Contains(search) ||
                t.LastName.ToLower().Contains(search) ||
                t.Email.ToLower().Contains(search) ||
                t.Expertise.ToLower().Contains(search)
             );

             _logger.LogInformation("Implemented Search Filtering");
        }

        if (!string.IsNullOrWhiteSpace(status.ToString()))
        {
            query = query.Where(t => string.Equals(t.Status.ToString(),status.ToString()));
             _logger.LogInformation("Implemented Status Filtering");
        }

        var mentors = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
             _logger.LogInformation("Implemented Pagination");

        // var mentors = await query.ToListAsync();
        return mentors.Select(MapResponse);

    }

    public async Task<MentorResponseDTO?> GetMentorById(int id)
    {

        var mentor = await database.Mentors.FindAsync(id);
        _logger.LogInformation("Get Mentor By Id Request Successful for Id No : {id}",id);
        return mentor == null ? null : MapResponse(mentor);
    }

    public async Task<MentorResponseDTO> CreateMentor(CreateMentorDTO request)
    {

        var mentor = new Mentor
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = request.Status,
            Expertise = request.Expertise,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now

        };

        await database.Mentors.AddAsync(mentor);

        await database.SaveChangesAsync();
        _logger.LogInformation("Mentor Created Succesfully");
        return MapResponse(mentor);

    }

    public async Task<MentorResponseDTO?> UpdateMentor(int id, UpdateMentorDTO request)
    {

        var mentor = await database.Mentors.FindAsync(id);

        if (mentor == null) return null;
        mentor.FirstName = request.FirstName;
        mentor.LastName = request.LastName;
        mentor.Email = request.Email;
        mentor.Status = request.Status;
        mentor.Expertise = request.Expertise;
        mentor.UpdatedDate = DateTime.Now;

        await database.SaveChangesAsync();
        _logger.LogInformation("Update Mentor Request Successful for Id No : {id}",id);

        return MapResponse(mentor);

    }

    public async Task<bool> DeleteMentor(int id)
    {
        var mentor = await database.Mentors.FindAsync(id);
        if (mentor == null) return false;
        database.Mentors.Remove(mentor);
        await database.SaveChangesAsync();
        _logger.LogInformation("Delete Mentor Successful for Id No : {id}",id);
        return true;
    }
    public MentorResponseDTO MapResponse(Mentor newMentor)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new MentorResponseDTO
        {
            Id = newMentor.Id,
            FirstName = newMentor.FirstName,
            LastName = newMentor.LastName,
            Email = newMentor.Email,
            Expertise = newMentor.Expertise,
            Status = newMentor.Status,
            CreatedDate = newMentor.CreatedDate,
            UpdatedDate = newMentor.UpdatedDate
        };


    }




}