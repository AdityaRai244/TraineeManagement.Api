using Microsoft.EntityFrameworkCore;

using TraineeManagement.SharedData.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ConsumerService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
});


var host = builder.Build();
host.Run();

