using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.SharedData.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TraineeManagement.SharedData.Models;

public class ConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? channel;
    public ConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            VirtualHost = _configuration["RabbitMQ:VirtualHost"],
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"],
        };

        try
        {
            Console.WriteLine(" Entered.");
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync("SubmissionProcessingExchange", ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue: "submission-processing", durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync("submission-processing", "SubmissionProcessingExchange", "SubmissionProcessingExchangeKey", null);
            Console.WriteLine("Connection has been created");

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                SubmissionProcessingRequested? payload = JsonSerializer.Deserialize<SubmissionProcessingRequested>(message, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null)
                {
                    throw new Exception("Received an empty or invalid message payload.");
                }

                try
                {

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var job = await db.ProcessingJob.FirstOrDefaultAsync(t => t.CorrelationId == payload.CorrelationId);

                        if (job.Status == JobStatus.Completed)
                        {
                            throw new Exception("Duplicate job received");
                        }

                        job.Status = JobStatus.Processing;
                        await db.SaveChangesAsync();

                        await processMessageAsync(payload);

                        job.Status = JobStatus.Completed;
                        Console.WriteLine("completed");
                        await db.SaveChangesAsync();
                    }


                    await channel.BasicAckAsync(ea.DeliveryTag, false);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);

                }

            };


            string consumerTag = await channel.BasicConsumeAsync("submission-processing", autoAck: false, consumer);

            Console.WriteLine(" Press control C  to exit.");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to the rabbitmq: {ex.Message}");
        }

    }

    public async Task processMessageAsync(SubmissionProcessingRequested? payload)
    {
        Console.WriteLine("Processing");
        await Task.Delay(5000);
        Console.WriteLine("Processing done");
        // payload.Status = JobStatus.Processing;
        // Console.WriteLine("prcoesssed");
        // await db.SaveChangesAsync();

    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (channel != null) await channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }


}