
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

    private static readonly Dictionary<string, string> SafeMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".pdf", "application/pdf" },
        { ".txt", "text/plain" },
    };


    public LocalFileStorageService(IConfiguration config, AppDbContext database, IHttpContextAccessor httpContextAccessor, ILogger<LocalFileStorageService> logger, ISubmissionProcessingService submissionProcessingService)
    {
        _database = database;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _submissionProcessingService = submissionProcessingService;
        _logger = logger;
    }


    private string GetStorageRootPath()
    {
        string basePath = _config["FileStorageService:Path"] ?? AppDomain.CurrentDomain.BaseDirectory;
        string absolutePath = Path.GetFullPath(basePath);

        if (!Directory.Exists(absolutePath))
        {
            Directory.CreateDirectory(absolutePath);
        }

        if (!absolutePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            absolutePath += Path.DirectorySeparatorChar;
        }

        return absolutePath;
    }


    private string GetSafeFilePath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BadRequestException("Invalid file name.");
        }

        string rootPath = GetStorageRootPath();

        // Combine and canonicalize the full path
        string combinedPath = Path.Combine(rootPath, fileName);
        string absoluteFilePath = Path.GetFullPath(combinedPath);

        // Path Traversal Mitigation: Ensure target path remains inside the base upload directory
        if (!absoluteFilePath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Path traversal attempt detected with filename: {FileName}", fileName);
            throw new BadRequestException("Invalid file path navigation.");
        }

        return absoluteFilePath;
    }


    public bool IsValidExtension(string extension)
    {
        var allowedExtensions = _config.GetSection("FileStorageService:AllowedExtensions").Get<List<string>>() ?? new List<string>();
        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
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

        if (!SafeMimeTypes.TryGetValue(extension, out string? safeContentType))
        {
            safeContentType = "application/octet-stream";
        }

        string storageName = $"{Guid.NewGuid()}{extension}";
        string filePath = GetSafeFilePath(storageName);

        await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream);
        }

        string calculatedCheckSum;
        await using (var checkSumStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = await sha256.ComputeHashAsync(checkSumStream);
            calculatedCheckSum = Convert.ToHexString(hashBytes);
        }

        string? claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(claimValue, out int userId))
        {
            var submissionFile = new SubmissionFile
            {
                SubmissionId = submissionId,
                OriginalFileName = file.FileName,
                StorageName = storageName,
                ContentType = safeContentType,
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
                StartedAt = DateTime.UtcNow
            };

            await _database.ProcessingJob.AddAsync(processingJobData);
            await _database.SaveChangesAsync();

            await _submissionProcessingService.PostSubmissionProcessingAsync(data);

            return $"/uploads/{storageName}";
        }
        else
        {
            _logger.LogError("User ID claim is missing or not a valid integer.");
            throw new UnauthorizedAccessException("Unauthorized user");
        }
    }


    public Task<bool> ExistsAsync(string fileName)
    {
        try
        {
            string filePath = GetSafeFilePath(fileName);
            return Task.FromResult(File.Exists(filePath));
        }
        catch (BadRequestException)
        {
            return Task.FromResult(false);
        }
    }

    public Task DeleteAsync(string fileName)
    {
        string filePath = GetSafeFilePath(fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("File {FileName} deleted successfully", fileName);
        }

        return Task.CompletedTask;
    }


    public Task<Stream> OpenReadAsync(string fileName)
    {
        string filePath = GetSafeFilePath(fileName);

        if (File.Exists(filePath))
        {
            // Return stream options optimized for async reading
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return Task.FromResult<Stream>(stream);
        }

        throw new NotFoundException("File does not exist");
    }

}