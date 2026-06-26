using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TraineeManagement.SharedData.Models;


public class SubmissionProcessingService : ISubmissionProcessingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SubmissionProcessingService> _logger;

    public SubmissionProcessingService(IConfiguration configuration, ILogger<SubmissionProcessingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
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

                var queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange","SubmissionFailedExchange"},
                {"x-dead-letter-routing-key","SubmissionFailedExchangeKey"}
            };

                await channel.ExchangeDeclareAsync("SubmissionProcessingExchange", ExchangeType.Direct);
                await channel.QueueDeclareAsync(queue: "submission-processing", durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
                await channel.QueueBindAsync("submission-processing", "SubmissionProcessingExchange", "SubmissionProcessingExchangeKey", null);
                _logger.LogInformation("Connection has been created");

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
                _logger.LogInformation($"{message} has been published");
            }



        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in rabbitmq: {ex.Message}");

        }

    }

}