using F1.Application.Services.Validation;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services;
using F1.TeamAPI.Services.Interfaces;
using F1.TeamAPI.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ConnectionDB>();

builder.Services.AddHttpClient("RaceAPI", client =>
    client.BaseAddress = new Uri("https://localhost:7001/api/"));

// Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IPilotRepository, PilotRepository>();
builder.Services.AddScoped<IEngineerRepository, EngineerRepository>();
builder.Services.AddScoped<IBossRepository, BossRepository>();

// Validators and other dependencies
builder.Services.AddScoped<TeamCreationValidator>();
builder.Services.AddScoped<TeamCountValidation>();
builder.Services.AddScoped<TeamPilotsValidator>();
builder.Services.AddScoped<TeamCarsValidator>();
builder.Services.AddScoped<CarEngineersValidator>();
builder.Services.AddScoped<TeamBossesValidator>();

// Service: use scoped (or register as typed client if you want HttpClient injected directly)
builder.Services.AddScoped<ITeamService, TeamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
