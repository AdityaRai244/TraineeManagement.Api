
namespace TraineeManagement.Api.Services;

using System.IO;
using TraineeManagement.SharedData.Data;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.SharedData.Models;

using System.Security.Cryptography;
using System.Security.Claims;
using System.Diagnostics;

class LocalFileStorageService : IFileStorageService
{

    private readonly IConfiguration _config;
    private readonly AppDbContext database;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISubmissionProcessingService submissionProcessingService;




    public LocalFileStorageService(IConfiguration config, AppDbContext database, IHttpContextAccessor httpContextAccessor, ISubmissionProcessingService submissionProcessingService)
    {
        this.database = database;
        this._config = config;
        this._httpContextAccessor = httpContextAccessor;
        this.submissionProcessingService = submissionProcessingService;
    }


    public Boolean IsValidExtension(string extension)
    {
        List<string> allowedExtensions = _config.GetSection("FileStorageService:AllowedExtensions").Get<List<string>>() ?? new List<string>();

        if (allowedExtensions.Contains(extension))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public async Task<string> SaveAsync(int submissionId, IFormFile file)
    {

        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No File Uploaded");
        }

        string extension = Path.GetExtension(file.FileName);

        if (!IsValidExtension(extension))
        {
            throw new BadRequestException("Invalid Format");
        }



        string maxSize = _config["FileStorageService:MaxSize"] ?? "5000000";

        if (!long.TryParse(maxSize, out long maxSizeBytes))
        {
            throw new BadRequestException("Invalid Max Size");
        }
        if (file.Length > maxSizeBytes)
        {
            throw new BadRequestException("File Too Big");
        }


        string? basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory; ;
        string absoluteBasePath = Path.GetFullPath(basePath);
        string folderPath = Path.Combine(absoluteBasePath, "uploads");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string ContentType = file.ContentType;

        //  if (!IsValidExtension(extension))
        // {
        //     throw new BadRequestException("Invalid Format");
        // }

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        string fileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        using var checkSumStream = file.OpenReadStream();
        using var sha256 = SHA256.Create();

        byte[] hashBytes = await sha256.ComputeHashAsync(checkSumStream);

        string calculatedCheckSum = Convert.ToHexString(hashBytes);

        string? claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(claimValue, out int userId))
        {

            var submissionFile = new SubmissionFile
            {
                SubmissionId = submissionId,
                OriginalFileName = file.FileName,
                StorageName = fileName,
                ContentType = ContentType,
                Size = file.Length,
                CheckSum = calculatedCheckSum,
                UploadedBy = userId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            await database.SubmissionFile.AddAsync(submissionFile);
            await database.SaveChangesAsync();

            var data = new SubmissionProcessingRequested
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                SubmissionId = submissionId,
                FileId = submissionFile.Id,
                RequestedAt = DateTime.UtcNow,
            };

            var processingJobData = new ProcessingJob
            {
                Attempts = 0,
                CorrelationId = data.CorrelationId,
                MessageId = data.MessageId,
                Status = JobStatus.Queued,
                StartedAt = DateTime.Now
            };

            await database.ProcessingJob.AddAsync(processingJobData);
            await database.SaveChangesAsync();


            await submissionProcessingService.PostSubmissionProcessingAsync(data);

            return $"/uploads/{fileName}";
        }
        else
        {
            Console.WriteLine("User ID claim is missing or not a valid integer.");
            throw new UnauthorizedAccessException("Unauthorized user");
        }



    }

    public Task<bool> ExistsAsync(string fileName)
    {

        string? basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory; ;
        string absoluteBasePath = Path.GetFullPath(basePath);

        string folderPath = Path.Combine(absoluteBasePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);

        return Task.FromResult(File.Exists(filePath));


    }

    public Task DeleteAsync(string fileName)
    {

        string? basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory; ;
        string absoluteBasePath = Path.GetFullPath(basePath);

        string folderPath = Path.Combine(absoluteBasePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);


        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;


    }

    public async Task<Stream> OpenReadAsync(string fileName)
    {

        string? basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory; ;
        string absoluteBasePath = Path.GetFullPath(basePath);


        string folderPath = Path.Combine(absoluteBasePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        }

        throw new BadRequestException("File does not exists");


    }

}