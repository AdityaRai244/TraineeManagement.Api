namespace TraineeManagement.Api.Services;

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraineeManagement.SharedData.Models;

public interface IFileStorageService
{
    Task<string> SaveAsync(int submissionId, IFormFile file);
    Task<bool> ExistsAsync(string fileName);
    Task DeleteAsync(string fileName);
    Task<Stream> OpenReadAsync(string fileName);
    Task<SubmissionFile?> GetFileMetadataAsync(int id);
    Task DeleteFileMetadataAsync(SubmissionFile metadata);
}
