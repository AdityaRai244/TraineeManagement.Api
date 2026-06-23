using TraineeManagement.Api.Models;

public interface ISubmissionProcessingService
{

    Task PostSubmissionProcessingAsync(SubmissionProcessingRequested request);

}