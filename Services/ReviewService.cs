using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

public class ReviewService : IReviewService
{

    private readonly AppDbContext database;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext database, ILogger<ReviewService> logger)
    {
        this.database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<ReviewResponseDTO>> GetAllReviews(ReviewStatus? status,  int pageNumber = 1, int pageSize = 10)
    {
        var query = database.Reviews.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status.ToString()))
        {
            query = query.Where(t => string.Equals(t.ReviewStatus.ToString(),status.ToString()));
             _logger.LogInformation("Implemented Status Filtering");
        }

        var Reviews = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
             _logger.LogInformation("Implemented Pagination");

        return Reviews.Select(MapResponse);

    }

    public async Task<ReviewResponseDTO?> GetReviewById(int id)
    {

        var Review = await database.Reviews.FindAsync(id);
        _logger.LogInformation("Get Review By Id Request Successful for Id No : {id}",id);
        return Review == null ? null : MapResponse(Review);
    }

    public async Task<ReviewResponseDTO> CreateReview(CreateReviewDTO request)
    {


        var review = new Review
        {
            SubmissionId = request.SubmissionId,
            MentorId = request.MentorId,
            Feedback = request.Feedback,
            Score = request.Score,
            ReviewStatus = request.ReviewStatus,
            ReviewedDate = request.ReviewedDate,

        };

        await database.Reviews.AddAsync(review);

        await database.SaveChangesAsync();
        _logger.LogInformation("Review Created Succesfully");
        return MapResponse(review);

    }

    public ReviewResponseDTO MapResponse(Review newReview)
    {

        _logger.LogInformation("Mapping Response to DTO");
        return new ReviewResponseDTO
        {
            Id = newReview.Id,
            SubmissionId = newReview.SubmissionId,
            MentorId = newReview.MentorId,
            Feedback = newReview.Feedback,
            Score = newReview.Score,
            ReviewStatus = newReview.ReviewStatus,
            ReviewedDate = newReview.ReviewedDate,

        };


    }




}