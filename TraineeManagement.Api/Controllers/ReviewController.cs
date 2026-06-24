using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/reviews")]

public class ReviewController : ControllerBase
{

    private readonly IReviewService reviewService;
    private readonly ILogger<ReviewController> _logger;
    public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
    {
        this.reviewService = reviewService;
        _logger = logger;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] ReviewStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
            return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var reviews = await reviewService.GetAllReviews(status, pageNumber, pageSize);
        _logger.LogInformation("Reviews fetched from service successfully");
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var review = await reviewService.GetReviewById(id);
        if (review == null)
        {
            _logger.LogError("Review with Id {id} Not found", id);
            return NotFound(new { message = $"Submission with {id} not found" });
        }
        _logger.LogInformation("Review with Id {id} Fetched from service Successfully", id);
        return Ok(review);

    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateReviewDTO request)
    {

        var review = await reviewService.CreateReview(request);
        _logger.LogInformation("Review Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);

    }

}