using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITraineeService,TraineeService>();
builder.Services.AddScoped<IAuthService,AuthService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();



// using(var scope = app.Services.CreateAsyncScope())
// {
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 
//    if (!db.Users.Any())
//    {
//       var admin = new User
//       {
//          Username = "Aditya",
//          Email = "aditya@gmail.com",
//          PasswordHash = "",
//          Role = UserRole.Admin
//       };
    //   var hasher = new PasswordHasher<User>();
//       string hashedPassword = hasher.HashPassword(admin, "pass");
//       admin.PasswordHash = hashedPassword;
//       Console.WriteLine("Seeding user: " + admin);
//       db.Users.Add(admin);
//       db.SaveChanges();
//    }
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
