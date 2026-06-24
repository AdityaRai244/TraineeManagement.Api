using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.SharedData.Models;

namespace TraineeManagement.Api.Services;

public class TraineeService : ITraineeService
{


    private readonly AppDbContext _database;
    private readonly ILogger<AuthService> _logger;
    private readonly IRedisService<TraineeResponseDTO> _redisCache;

    public TraineeService(IRedisService<TraineeResponseDTO> redisCache, AppDbContext database, ILogger<AuthService> logger)
    {
        _database = database;
        _logger = logger;
        _redisCache = redisCache;
    }

    public async Task<PagedResponseDTO> GetAllTrainees(UserStatus? status, string? search = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _database.Trainees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(t =>
                t.FirstName.ToLower().Contains(search) ||
                t.LastName.ToLower().Contains(search) ||
                t.Email.ToLower().Contains(search) ||
                t.TechStack.ToLower().Contains(search)
             );

            _logger.LogInformation("Implemented Search Filtering");
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
            _logger.LogInformation("Implemented Status Filtering");
        }


        int count = await query.CountAsync();
        var trainees = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        _logger.LogInformation("Implemented Pagination");

        var response = new PagedResponseDTO
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = count,
            Data = trainees
        };
        return response;

    }

    public async Task<TraineeResponseDTO?> GetTraineeById(int id)
    {

        string idString = id.ToString();
        var trainee = await _redisCache.GetAsync(idString);

        if(trainee is not null)
        {
            _logger.LogInformation("Cache HIT for trainee {Id}", id);
            return trainee;
        }

        _logger.LogWarning("Cache MISS for trainee {Id}. Accessing DB.", id);
        var dbTrainee = await _database.Trainees.FindAsync(id);
        if (dbTrainee == null)
        {
            throw new NotFoundException("Trainee");
        }

         TraineeResponseDTO mappedResponse = MapResponse(dbTrainee);
        await _redisCache.SetAsync(idString, mappedResponse);
         _logger.LogInformation("Cache updated for Trainee {Id}", id);
        _logger.LogInformation("Get Trainee By Id Request Successful for Id No : {id}", id);
      
        return mappedResponse;

    }

    public async Task<TraineeResponseDTO> CreateTrainee(CreateTraineeRequestDTO request)
    {

        var trainee = new Trainee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = request.Status,
            TechStack = request.TechStack,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow

        };

        await _database.Trainees.AddAsync(trainee);

        await _database.SaveChangesAsync();
        _logger.LogInformation("Trainee Created Succesfully");
        return MapResponse(trainee);

    }

    public async Task<TraineeResponseDTO?> UpdateTrainee(int id, UpdateTraineeRequestDTO request)
    {

        var trainee = await _database.Trainees.FindAsync(id);
        if (trainee == null)
        {
            throw new NotFoundException("Trainee");
        }

        if (trainee == null) return null;
        trainee.FirstName = request.FirstName;
        trainee.LastName = request.LastName;
        trainee.Email = request.Email;
        trainee.Status = request.Status;
        trainee.TechStack = request.TechStack;
        trainee.UpdatedDate = DateTime.UtcNow;

        await _database.SaveChangesAsync();
        _logger.LogInformation("Update Trainee Request Successful for Id No : {id}", id);

        await _redisCache.RemoveAsync(id.ToString());

        _logger.LogInformation("Cached Updated for Trainee Id No : {id}", id);


        return MapResponse(trainee);

    }

    public async Task<bool> DeleteTrainee(int id)
    {
        var trainee = await _database.Trainees.FindAsync(id);
        if (trainee == null)
        {
            throw new NotFoundException("Trainee");
        }
        _database.Trainees.Remove(trainee);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Delete Trainee Successful for Id No : {id}", id);

        await _redisCache.RemoveAsync(id.ToString());
        _logger.LogInformation("Cache removed for Trainee Id : {id}", id);

        return true;
    }
    public TraineeResponseDTO MapResponse(Trainee newTrainee)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new TraineeResponseDTO
        {
            Id = newTrainee.Id,
            FirstName = newTrainee.FirstName,
            LastName = newTrainee.LastName,
            Email = newTrainee.Email,
            TechStack = newTrainee.TechStack,
            Status = newTrainee.Status,
            CreatedDate = newTrainee.CreatedDate,
            UpdatedDate = newTrainee.UpdatedDate
        };


    }


}