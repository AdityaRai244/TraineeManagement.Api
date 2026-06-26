using httpClient.services;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddControllers(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapControllers(); 


app.UseHttpsRedirection();

app.Run();


// curl -X GET https://localhost:<your-port>/api/trainee