using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;

public class MentorService : IMentorService
{



    private readonly AppDbContext _database;
    private readonly ILogger<AuthService> _logger;

    public MentorService(AppDbContext database, ILogger<AuthService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<MentorResponseDTO>> GetAllMentors(MentorStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _database.Mentors.AsQueryable();

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

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
                _logger.LogInformation("Implemented Status Filtering");
        }

        var mentors = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        _logger.LogInformation("Implemented Pagination");

        return mentors.Select(MapResponse);

    }

    public async Task<MentorResponseDTO?> GetMentorById(int id)
    {

        var mentor = await _database.Mentors.FindAsync(id);
        if (mentor == null)
        {
            throw new NotFoundException("Mentor");
        }
        _logger.LogInformation("Get Mentor By Id Request Successful for Id No : {id}", id);
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
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow

        };

        await _database.Mentors.AddAsync(mentor);

        await _database.SaveChangesAsync();
        _logger.LogInformation("Mentor Created Succesfully");
        return MapResponse(mentor);

    }

    public async Task<MentorResponseDTO?> UpdateMentor(int id, UpdateMentorDTO request)
    {

        var mentor = await _database.Mentors.FindAsync(id);

        if (mentor == null)
        {
            throw new NotFoundException("Mentor");
        }
        mentor.FirstName = request.FirstName;
        mentor.LastName = request.LastName;
        mentor.Email = request.Email;
        mentor.Status = request.Status;
        mentor.Expertise = request.Expertise;
        mentor.UpdatedDate = DateTime.UtcNow;

        await _database.SaveChangesAsync();
        _logger.LogInformation("Update Mentor Request Successful for Id No : {id}", id);

        return MapResponse(mentor);

    }

    public async Task<bool> DeleteMentor(int id)
    {
        var mentor = await _database.Mentors.FindAsync(id);
        if (mentor == null)
        {
            throw new NotFoundException("Mentor");
        }
        ;
        _database.Mentors.Remove(mentor);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Delete Mentor Successful for Id No : {id}", id);
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