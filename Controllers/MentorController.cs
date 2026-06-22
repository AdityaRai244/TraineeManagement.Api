using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/mentors")]

public class MentorController : ControllerBase
{
    
    private readonly IMentorService mentorService;
    // private readonly IRedisService<Mentor> redisService;

    private readonly ILogger<MentorController> _logger;
    public MentorController(
        // IRedisService<Mentor> redisService,
        IMentorService mentorService, ILogger<MentorController> logger)
    {
        // this.redisService = redisService;
        this.mentorService = mentorService;
        _logger = logger;

    }

     [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get( [FromQuery] MentorStatus? status, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid Paramters");
		    return BadRequest($"{nameof(pageNumber)} and {nameof(pageSize)} size must be greater than 0.");
        }

        var mentors = await mentorService.GetAllMentors(status,search,pageNumber,pageSize);
        _logger.LogInformation("Mentors fetched from service successfully");
        return Ok(mentors);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(int id)
    {

        var mentor = await mentorService.GetMentorById(id);
        if(mentor == null)
        {
            _logger.LogError("Mentor with Id {id} Not found",id);
            return NotFound(new {message = $"Mentor with {id} not found"});
        }
        _logger.LogInformation("Mentor with Id {id} Fetched from service Successfully",id);
        return Ok(mentor);

    }

    
    // [HttpGet("{id}/redis")]
    // [Authorize]
    // public async Task<ActionResult> GetRedisCheck(int id)
    // {

    //     var mentor = await redisService.GetAsync("asdfasdf");
    //     // if(mentor == null)
    //     // {
    //     //     _logger.LogError("Mentor with Id {id} Not found",id);
    //     //     return NotFound(new {message = $"Mentor with {id} not found"});
    //     // }
    //     // _logger.LogInformation("Mentor with Id {id} Fetched from service Successfully",id);
    //     return Ok(mentor);

    // }

    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] CreateMentorDTO request)
    {

        var mentor = await mentorService.CreateMentor(request);
        _logger.LogInformation("Mentor Created From Service Successfully");
        return CreatedAtAction(nameof(GetById), new {id = mentor.Id}, mentor);

    }

     [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Put(int id, [FromBody] UpdateMentorDTO request)
    {
        var mentor = await mentorService.UpdateMentor(id, request);
        if(mentor == null)
        {
            _logger.LogError("Mentor with Id {id} Not found",id);
            return NotFound(new {message = $"Mentor with {id} not found"});
        }
        _logger.LogInformation("Mentor Updated From Service Successfully. ID : {id}",id);
        return Ok(mentor);

    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await mentorService.DeleteMentor(id);
        if (!success)
        {
            _logger.LogError("Mentor with Id {id} Not found",id);
            return NotFound(new {message = $"Mentor with {id} not found"});
        }
        _logger.LogInformation("Mentor Deleted From Service Successfully. ID : {id}",id);
        return NoContent();
    }

}