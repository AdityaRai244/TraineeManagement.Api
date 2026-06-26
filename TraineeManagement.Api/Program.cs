using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.SharedData.Data;
using TraineeManagement.SharedData.Models;

using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using TraineeManagement.Api.Exceptions;
using RabbitMQ.Client;                              
using System;    

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
}); ;


// ---------AUTHENTICATION IN SWAGGER-----------

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TraineeManagement.Api",
        Version = "v1"
    });

    // Define security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token status below.",

    });

    // Apply security to endpoints
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped(typeof(IRedisService<>), typeof(RedisService<>));
builder.Services.AddSingleton<ISubmissionProcessingService, SubmissionProcessingService>();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var rabbitSection = builder.Configuration.GetSection("RabbitMQ");


builder.Services.AddHealthChecks()
    .AddMySql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "mysql-db",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["database", "mysql"])
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: ["ready"])
    .AddRabbitMQ(
        async sp =>
        {
            var factory = new ConnectionFactory
            {
               HostName = rabbitSection["Host"],
                Port = int.Parse(rabbitSection["Port"] ?? "5672"),
                VirtualHost = rabbitSection["VirtualHost"] ?? "/",
                UserName = rabbitSection["Username"], 
                Password = rabbitSection["Password"],
            };
            return await factory.CreateConnectionAsync();
        },
        name: "rabbitmq",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "mq", "rabbit" }
    )
    .AddUrlGroup(
        uri: new Uri("http://localhost:5287/api/health"),
        name : "TraineeDirectory.Api",
        failureStatus : HealthStatus.Unhealthy,
        timeout : TimeSpan.FromSeconds(10)
    );






// ---------CORS-------------------

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:5173")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod();
                      });
});


//----------------- JWT ----------------


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
            };
        });

builder.Services.AddHttpContextAccessor();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});




// Configure the HTTP request pipeline.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });

    // app.UseExceptionHandler(options =>
    // {
    //     options.Run(async context =>
    //     {
    //         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //         context.Response.ContentType = "application/json";


    //         var exceptionHandlerPathFeature =
    //             context.Features.Get<IExceptionHandlerPathFeature>();

    //         if(exceptionHandlerPathFeature is not null)
    //         {
    //             var error = new {message = "An unexpected error occured"};
    //             await context.Response.WriteAsJsonAsync(error);
    //         }
    //     });
    // });

}



// Readiness — checks everything needed to serve requests
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(result);

    }
});

// -----------SEED USER ----------

// using(var scope = app.Services.CreateAsyncScope())
// {
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    if (!db.Users.Any())
//    {
//       var admin = new User
//       {
//          Username = "string",
//          Email = "aditya@gmail.com",
//          PasswordHash = "",
//          Role = UserRole.Admin
//       };
//       var hasher = new PasswordHasher<User>();
//       string hashedPassword = hasher.HashPassword(admin, "string");
//       admin.PasswordHash = hashedPassword;
//       Console.WriteLine("Seeding user: " + admin);
//       db.Users.Add(admin);
//       db.SaveChanges();
//    }
// }

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();
app.UseDefaultFiles();

app.Run();
