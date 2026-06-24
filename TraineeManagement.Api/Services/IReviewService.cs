namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;


public interface IReviewService
{
    
    Task<IEnumerable<ReviewResponseDTO>> GetAllReviews(ReviewStatus? status,int pageNumber = 1, int pageSize = 10);
    Task<ReviewResponseDTO?> GetReviewById(int id);
    Task<ReviewResponseDTO> CreateReview(CreateReviewDTO task);

}