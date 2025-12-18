using F1.CompetitionAPI.Data;
using F1.CompetitionAPI.Repositories;
using F1.CompetitionAPI.Repositories.Interfaces;
using F1.CompetitionAPI.Services;
using F1.CompetitionAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ConnectionDB>();
builder.Services.AddSingleton<ICompetitionService, CompetitionService>();
builder.Services.AddSingleton<ICompetitionRepository, CompetitionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
