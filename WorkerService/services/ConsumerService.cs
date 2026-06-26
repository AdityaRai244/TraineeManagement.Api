using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.SharedData.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TraineeManagement.SharedData.Models;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace WorkerService.Services;

public class ConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConsumerService> _logger;

    private readonly HttpClient _client;
    private IConnection? _connection;
    private IChannel? channel;

    public ConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<ConsumerService> logger, HttpClient client)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _client = client;
        _logger = logger;
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
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync("SubmissionFailedExchange", ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue: "submission-failed", durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync("submission-failed", "SubmissionFailedExchange", "SubmissionFailedExchangeKey", null);

            var queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange","SubmissionFailedExchange"},
                {"x-dead-letter-routing-key","SubmissionFailedExchangeKey"}
            };

            await channel.ExchangeDeclareAsync("SubmissionProcessingExchange", ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue: "submission-processing", durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            await channel.QueueBindAsync("submission-processing", "SubmissionProcessingExchange", "SubmissionProcessingExchangeKey", null);

            _logger.LogInformation("Connection has been created");

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
                _logger.LogInformation($"Correlation Id Received : {payload.CorrelationId}");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var job = await db.ProcessingJob.FirstOrDefaultAsync(t => t.MessageId == payload.MessageId, stoppingToken);
                    if (job == null)
                    {
                        await channel.BasicRejectAsync(ea.DeliveryTag, false);
                        return;
                    }
                    if (job.Status == JobStatus.Completed)
                    {
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    try
                    {


                        var SubmissionFile = await db.SubmissionFile
         .FirstOrDefaultAsync(f => f.Id == payload.FileId);

                        //     var isDuplicate = await db.SubmissionFile
                        //   .AnyAsync(f => f.CheckSum == SubmissionFile.CheckSum && f.Id != SubmissionFile.Id);


                        //     if (isDuplicate)
                        //     {
                        //         Console.WriteLine($"Skipping processing. File with checksum {SubmissionFile.CheckSum} already processed.");

                        //         job.Status = JobStatus.Completed;
                        //         await db.SaveChangesAsync();
                        //         await channel.BasicAckAsync(ea.DeliveryTag, false);
                        //         return;
                        //     }

                        job.Attempts += 1;
                        job.Status = JobStatus.Processing;
                        await db.SaveChangesAsync(stoppingToken);

                        await processMessageAsync(payload, db, stoppingToken);
                        // throw new Exception();

                        job.Status = JobStatus.Completed;
                        await db.SaveChangesAsync(stoppingToken);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        if (job.Attempts >= 3)
                        {
                            _logger.LogError("Failed to process file. All Attempts exhausted.");
                            job.Status = JobStatus.Failed;
                            await db.SaveChangesAsync(stoppingToken);
                            await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                        else
                        {
                            _logger.LogInformation("Failed to process file. Requeuing to Rabbit MQ");
                            await db.SaveChangesAsync(stoppingToken);
                            await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                }
                ;
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
            _logger.LogError($"Could not connect to the rabbitmq: {ex.Message}");
        }

    }

    public async Task<string> processMessageAsync(SubmissionProcessingRequested payload, AppDbContext db, CancellationToken cancellationToken)
    {

        int fileId = payload.FileId;
        SubmissionFile submissionFile = await db.SubmissionFile.FirstOrDefaultAsync(t => t.Id == fileId);

        var userId = await db.SubmissionFile
        .Where(sf => sf.Id == fileId)
        .Select(sf => sf.Submission.TaskAssignment.TraineeId)
        .FirstOrDefaultAsync();


        if (submissionFile == null)
        {
            _logger.LogError("File does not exists");
            throw new Exception("File does not exists");
        }
        string configuredPath = _configuration["FileStorageService:Path"] ?? "uploads";
        string finalPath = Path.Combine(AppContext.BaseDirectory, configuredPath, submissionFile.StorageName);
        string basePath = Path.GetFullPath(finalPath);
        await using var fileStream = new FileStream(basePath, FileMode.Open, FileAccess.Read);
        using var sha256 = SHA256.Create();
        byte[] hashBytes = await sha256.ComputeHashAsync(fileStream);
        string calculatedCheckSum = Convert.ToHexString(hashBytes);

        if (calculatedCheckSum == submissionFile.CheckSum)
        {
            _logger.LogInformation("Checksum validated successfully");
            using HttpResponseMessage response = await _client.GetAsync($"trainees/{userId}/{payload.CorrelationId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Resposne received from TraineeDirectory.Api Sucessfully", responseBody);
            return responseBody;
        }
        else
        {
            _logger.LogError("File has been corrupted");
            throw new Exception("File has been corrupted");
        }


    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (channel != null) await channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }


}