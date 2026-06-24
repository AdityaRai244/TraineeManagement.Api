using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TraineeManagement.SharedData.Models;


public class SubmissionProcessingService : ISubmissionProcessingService
{
    private readonly IConfiguration _configuration;

    public SubmissionProcessingService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PostSubmissionProcessingAsync(SubmissionProcessingRequested request)
    {

        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:Host"]!,
            Port = int.Parse(_configuration["RabbitMQ:Port"]!),
            VirtualHost = _configuration["RabbitMQ:VirtualHost"]!,
            UserName = _configuration["RabbitMQ:Username"]!,
            Password = _configuration["RabbitMQ:Password"]!,
        };

        try
        {
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {

                await channel.ExchangeDeclareAsync("SubmissionProcessingExchange", ExchangeType.Direct);
                await channel.QueueDeclareAsync(queue: "submission-processing", durable: true, exclusive: false, autoDelete: false, arguments: null);
                await channel.QueueBindAsync("submission-processing", "SubmissionProcessingExchange", "SubmissionProcessingExchangeKey", null);
                Console.WriteLine("Connection has been created");

                var message = JsonSerializer.Serialize(request);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties
                {
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: "SubmissionProcessingExchange",
                    routingKey: "SubmissionProcessingExchangeKey",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );
                Console.WriteLine($"{message} has been sent");
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to the rabbitmq: {ex.Message}");

        }

    }

}