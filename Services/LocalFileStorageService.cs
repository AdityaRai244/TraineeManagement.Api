
namespace TraineeManagement.Api.Services;

using System.IO;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Models;
using System.Security.Cryptography;
using System.Security.Claims;

class LocalFileStorageService : IFileStorageService
{

    private readonly IConfiguration _config;
    private readonly AppDbContext database;
    private readonly IHttpContextAccessor _httpContextAccessor;



    public LocalFileStorageService(IConfiguration config, AppDbContext database, IHttpContextAccessor httpContextAccessor)
    {
        this.database = database;
        this._config = config;
        this._httpContextAccessor = httpContextAccessor;
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


        string? basePath = _config["FileStorageService:Path"]!;
        basePath = basePath.Replace('/', '\\').Trim();

        if (basePath.Contains(":"))
        {
            char driveLetter = char.ToLower(basePath[0]);
            string rest = basePath.Substring(2).Replace('\\', '/');
            basePath = $"/mnt/{driveLetter}{rest}";
        }


        string folderPath = Path.Combine(basePath, "uploads");
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

        string? basePath = _config["FileStorageService:Path"]!;
        basePath = basePath.Replace('/', '\\').Trim();

        if (basePath.Contains(":"))
        {
            char driveLetter = char.ToLower(basePath[0]);
            string rest = basePath.Substring(2).Replace('\\', '/');
            basePath = $"/mnt/{driveLetter}{rest}";
        }
        string folderPath = Path.Combine(basePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);

        return Task.FromResult(File.Exists(filePath));


    }

    public Task DeleteAsync(string fileName)
    {

        string? basePath = _config["FileStorageService:Path"]!;
        basePath = basePath.Replace('/', '\\').Trim();

        if (basePath.Contains(":"))
        {
            char driveLetter = char.ToLower(basePath[0]);
            string rest = basePath.Substring(2).Replace('\\', '/');
            basePath = $"/mnt/{driveLetter}{rest}";
        }
        string folderPath = Path.Combine(basePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);


        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;


    }

    public async Task<Stream> OpenReadAsync(string fileName)
    {

        string? basePath = _config["FileStorageService:Path"]!;
        basePath = basePath.Replace('/', '\\').Trim();

        if (basePath.Contains(":"))
        {
            char driveLetter = char.ToLower(basePath[0]);
            string rest = basePath.Substring(2).Replace('\\', '/');
            basePath = $"/mnt/{driveLetter}{rest}";
        }
        string folderPath = Path.Combine(basePath, "uploads");
        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        }

        throw new BadRequestException("File does not exists");


    }

}