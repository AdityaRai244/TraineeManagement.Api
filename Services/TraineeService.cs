using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services;

public class TraineeService : ITraineeService
{
    
    private static readonly List<Trainee> trainees = new List<Trainee>();
    private static  int traineeId = 1;
    public IEnumerable<TraineeResponse> GetAllTrainees()
    {
        return trainees.Select(MapResponse);
    }

    public TraineeResponse? GetTraineeById(int id)
    {
        var trainee = trainees.Find(t => t.Id == id);
        return trainee == null ? null : MapResponse(trainee); 
    }

    public TraineeResponse CreateTrainee(CreateTraineeRequest request)
    {
        
        var trainee = new Trainee
        {
            Id = traineeId++,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = request.Status,
            TechStack = request.TechStack,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now

        };

        trainees.Add(trainee);
        return MapResponse(trainee);

    }

    public TraineeResponse? UpdateTrainee(int id, UpdateTraineeRequest request)
    {

        var trainee = trainees.Find(t => t.Id == id);
        if(trainee == null) return null;
        trainee.FirstName = request.FirstName;
        trainee.LastName = request.LastName;
        trainee.Email = request.Email;
        trainee.Status = request.Status;
        trainee.TechStack = request.TechStack;
        trainee.UpdatedDate = DateTime.Now;

        return MapResponse(trainee);

    }

   

    public bool DeleteTrainee(int id)
    {
        var trainee = trainees.Find(t => t.Id == id);
        if(trainee == null) return false;
        trainees.Remove(trainee);
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