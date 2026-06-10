using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Services;

public class TraineeService : ITraineeService
{
    

    private readonly AppDbContext database;
    public TraineeService(AppDbContext database){
        this.database = database;
    }

    public async Task<IEnumerable<TraineeResponse>> GetAllTrainees(string? search = null)
    {
        var query =  database.Trainees.AsQueryable();
        if(!string.IsNullOrWhiteSpace(search)){
            search = search.ToLower();

            query = query.Where(t =>
                t.FirstName.ToLower().Contains(search) ||
                t.LastName.ToLower().Contains(search) ||
                t.Email.ToLower().Contains(search) ||
                t.TechStack.ToLower().Contains(search)
             );
        }

             var trainees = await query.ToListAsync();
            return trainees.Select(MapResponse);

    }

    public async Task<TraineeResponse?> GetTraineeById(int id)
    {

        var trainee = await database.Trainees.FindAsync(id);
        return trainee == null ? null : MapResponse(trainee); 
    }

    public async Task<TraineeResponse> CreateTrainee(CreateTraineeRequest request)
    {
        
        var trainee = new Trainee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = request.Status,
            TechStack = request.TechStack,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now

        };

        await database.Trainees.AddAsync(trainee);

        await database.SaveChangesAsync();
        return MapResponse(trainee);

    }

    public async Task<TraineeResponse?> UpdateTrainee(int id, UpdateTraineeRequest request)
    {

        var trainee = await database.Trainees.FindAsync(id);

        if(trainee == null) return null;
        trainee.FirstName = request.FirstName;
        trainee.LastName = request.LastName;
        trainee.Email = request.Email;
        trainee.Status = request.Status;
        trainee.TechStack = request.TechStack;
        trainee.UpdatedDate = DateTime.Now;

        await database.SaveChangesAsync();

        return MapResponse(trainee);

    }

    public async Task<bool> DeleteTrainee(int id)
    {
        var trainee = await database.Trainees.FindAsync(id);
        if(trainee == null) return false;
        database.Trainees.Remove(trainee);
        await database.SaveChangesAsync();
        return true;
    }
     public TraineeResponse MapResponse(Trainee newTrainee)
    {
        
        return new TraineeResponse {
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