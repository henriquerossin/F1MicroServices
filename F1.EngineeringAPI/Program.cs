using F1.EngineeringAPI.Services;
using F1.EngineeringAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient("TeamAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:8001/api");
});

builder.Services.AddSingleton<IEngineeringService, EngineeringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
