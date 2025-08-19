using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configurar Entity Framework
builder.Services.AddDbContext<WorkTrackBioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Remover el ejemplo de WeatherForecast ya que no lo necesitamos
// app.MapGet("/weatherforecast", () => { ... });

app.Run();
