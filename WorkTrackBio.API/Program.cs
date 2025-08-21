using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Services.StateService;
using WorkTrackBio.API.Validators.StateValidator;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Services.RoleService;
using WorkTrackBio.API.Validators.RoleValidator;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers - NECESARIO para que funcionen los controladores
builder.Services.AddControllers();

// Configurar Entity Framework
builder.Services.AddDbContext<WorkTrackBioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Repositories
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Register Validators
builder.Services.AddScoped<IStateValidator, StateValidator>();
builder.Services.AddScoped<IRoleValidator, RoleValidator>();

// Register Services
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<IRoleService, RoleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Deshabilitar completamente la redirección HTTPS para desarrollo
app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});

// Map Controllers - NECESARIO para que funcionen las rutas de los controladores
app.MapControllers();

// Remover el ejemplo de WeatherForecast ya que no lo necesitamos
// app.MapGet("/weatherforecast", () => { ... });

app.Run();
