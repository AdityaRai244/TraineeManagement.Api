using TraineeManagement.SharedData.Models;


public interface ISubmissionProcessingService
{

    Task PostSubmissionProcessingAsync(SubmissionProcessingRequested request);

}