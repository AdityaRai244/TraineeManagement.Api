using Microsoft.AspNetCore.Mvc;
namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("/api/trainees")]
public class TraineeController : ControllerBase
{
    private static int Id = 1;
    private static readonly List<Trainee> trainees = new List<Trainee>();

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(trainees);
    }

    [HttpGet("{id}")]
    public ActionResult GetById(int id)
    {
        var trainee = trainees.Find(t => t.Id == id);
        if(trainee == null)
        {
            return NotFound(new {status = "Could not found the trainee"});
        }
        else
        {
            return Ok(trainee);
        }

    }
    

    [HttpPost]
    public ActionResult Post([FromBody] Trainee newTrainee)
    {
        
        newTrainee.Id = Id++;
        newTrainee.CreatedDate = DateTime.Now;
        newTrainee.UpdatedDate = DateTime.Now;
        trainees.Add(newTrainee);

        return Ok(new {msg = "Trainee added successfully"});

    }
}