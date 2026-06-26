using System.Text.Json;
using System.Net.Http.Json;
namespace httpClient.services;

public class TraineeService : ITraineeService
{

    public class Trainee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
    }


    public string GetUserDataAsync()
    {


        try
        {
            var trainee = new Trainee()
            {
                Id = 1,
                FirstName = "Aditya",
                LastName = "Rai",
                Email = "aditya@gmail.com",
                TechStack = ".Net, React.Js"
            };
            string jsonString = JsonSerializer.Serialize(trainee);
            return jsonString;
        }
        catch (Exception ex)
        {
            Console.Write(ex);
            return "failed";
        }
    }


}