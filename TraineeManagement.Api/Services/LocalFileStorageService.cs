
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
    private readonly AppDbContext _database;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly ISubmissionProcessingService _submissionProcessingService;




    public LocalFileStorageService(IConfiguration config, AppDbContext database, IHttpContextAccessor httpContextAccessor, ILogger<LocalFileStorageService> logger, ISubmissionProcessingService submissionProcessingService)
    {
        _database = database;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _submissionProcessingService = submissionProcessingService;
        _logger = logger;
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
            _logger.LogError("No file uploaded");
            throw new BadRequestException("No File Uploaded");
        }

        string extension = Path.GetExtension(file.FileName);

        if (!IsValidExtension(extension))
        {
            _logger.LogError("Invalid file extension");
            throw new BadRequestException("Invalid Format");
        }



        string maxSize = _config["FileStorageService:MaxSize"] ?? "5000000";

        if (!long.TryParse(maxSize, out long maxSizeBytes))
        {
            _logger.LogError("Max size parsing failed");
            throw new BadRequestException("Invalid Max Size");
        }
        if (file.Length > maxSizeBytes)
        {
            _logger.LogError("Uploaded file is too big.");
            throw new BadRequestException("Uploaded file is too big.");
        }


        string? basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory; ;
        string absoluteBasePath = Path.GetFullPath(basePath);
        string folderPath = Path.Combine(absoluteBasePath, "uploads");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string ContentType = file.ContentType;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        string fileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        await using var checkSumStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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
            await _database.SubmissionFile.AddAsync(submissionFile);
            await _database.SaveChangesAsync();

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

            await _database.ProcessingJob.AddAsync(processingJobData);
            await _database.SaveChangesAsync();


            await _submissionProcessingService.PostSubmissionProcessingAsync(data);

            return $"/uploads/{fileName}";
        }
        else
        {
            _logger.LogError("User ID claim is missing or not a valid integer.");
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
            _logger.LogInformation("File deleted successfully");
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