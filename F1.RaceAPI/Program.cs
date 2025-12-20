using F1.RaceAPI.Data;
using F1.RaceAPI.Repositories;
using F1.RaceAPI.Repositories.Interfaces;
using F1.RaceAPI.Services;
using F1.RaceAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<ConnectionDB>();

builder.Services.AddHttpClient<RaceService>(client => client.BaseAddress = new Uri("https://localhost:5001/api/Competition/"));

builder.Services.AddSingleton<IRaceService, RaceService>();

builder.Services.AddSingleton<IRaceRepository, RaceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
