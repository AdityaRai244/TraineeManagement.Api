using System.Text.Json;
using System.Net.Http.Json;
namespace httpClient.services;

public class TraineeService : ITraineeService
{


    private readonly List<Trainee> _dummyTrainees = new()
    {
        new Trainee { Id = 1, FirstName = "Aditya", LastName = "Rai", Email = "aditya@gmail.com", TechStack = ".Net, React.Js" },
        new Trainee { Id = 2, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", TechStack = "Java, Angular" },
        new Trainee { Id = 3, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", TechStack = "Python, Vue.Js" }
    };


    public class Trainee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
    }


    public string GetUserDataAsync(int id)
    {


        try
        {
            if (id <= 0)
            {
                return "failed: Invalid ID";
            }

            var trainee = _dummyTrainees.FirstOrDefault(t => t.Id == id);
            if (trainee == null)
            {
                return $"failed: Trainee with ID {id} not found";
            }
            string jsonString = JsonSerializer.Serialize(trainee);
            return jsonString;
        }
        catch (Exception ex)
        {
            Console.Write(ex);
            return "Failed : An Unexpected Error Occurred";
        }
    }


}