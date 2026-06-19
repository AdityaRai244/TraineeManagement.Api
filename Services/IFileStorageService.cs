namespace TraineeManagement.Api.Services;

public interface IFileStorageService
{
    
    Task<string> SaveAsync(int submissionId, IFormFile file);
    Task<bool> ExistsAsync(string fileName);
    Task DeleteAsync(string fileName);
    Task<Stream> OpenReadAsync(string fileName);


}