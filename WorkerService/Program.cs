

using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using TraineeManagement.SharedData.Data;
using WorkerService.Services;
using System.Web;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using WorkerService.TryName;


var builder = Host.CreateApplicationBuilder(args);
var serverVersion = new MySqlServerVersion(new Version(9,7,0));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddSingleton<ConsumerService>();

builder.Services.AddHttpClient<ConsumerService>((serviceProvider, client) =>
    {

        client.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:HttpClientAPI"]);
    })
     .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        };
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
    .AddHttpMessageHandler(() => new HttpStatusCodeFallbackHandler())
    .AddStandardResilienceHandler(options =>
{

    options.Retry.MaxRetryAttempts = 5;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.Retry.UseJitter = true;

    options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
        .Handle<HttpRequestException>()
        .HandleResult(response =>
        response.StatusCode == HttpStatusCode.RequestTimeout ||
        response.StatusCode == HttpStatusCode.ServiceUnavailable ||
        response.StatusCode == HttpStatusCode.TooManyRequests ||
        (int)response.StatusCode >= 500
        );

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(20);
    options.CircuitBreaker.MinimumThroughput = 5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHostedService(sp => sp.GetRequiredService<ConsumerService>());






var host = builder.Build();
host.Run();


