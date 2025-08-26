using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Services.StateService;
using WorkTrackBio.API.Validators.StateValidator;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Services.RoleService;
using WorkTrackBio.API.Validators.RoleValidator;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Services.ProjectService;
using WorkTrackBio.API.Validators.ProjectValidator;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;
using WorkTrackBio.API.Services.DocumentTypeService;
using WorkTrackBio.API.Validators.DocumentTypeValidator;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Services.EmployeeInfoService;
using WorkTrackBio.API.Validators.EmployeeInfoValidator;
using WorkTrackBio.API.Validators.DocumentValidator;
using WorkTrackBio.API.Validators.PhoneNumberFormatter;
using WorkTrackBio.API.Repositories.InternUserRepository;
using WorkTrackBio.API.Services.InternUserService;
using WorkTrackBio.API.Validators.InternUserValidator;
using WorkTrackBio.API.Repositories.ProjectMaintenanceRepository;
using WorkTrackBio.API.Services.ProjectMaintenanceService;
using WorkTrackBio.API.Validators.ProjectMaintenanceValidator;
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
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        builder.Services.AddScoped<IEmployeeInfoRepository, EmployeeInfoRepository>();
        builder.Services.AddScoped<IInternUserRepository, InternUserRepository>();
        builder.Services.AddScoped<IProjectMaintenanceRepository, ProjectMaintenanceRepository>();

        // Register Validators
        builder.Services.AddScoped<IStateValidator, StateValidator>();
        builder.Services.AddScoped<IRoleValidator, RoleValidator>();
        builder.Services.AddScoped<IProjectValidator, ProjectValidator>();
        builder.Services.AddScoped<IDocumentTypeValidator, DocumentTypeValidator>();
        builder.Services.AddScoped<IEmployeeInfoValidator, EmployeeInfoValidator>();
        builder.Services.AddScoped<IDocumentValidator, DocumentValidator>();
        builder.Services.AddScoped<IPhoneNumberFormatter, PhoneNumberFormatter>();
        builder.Services.AddScoped<IInternUserValidator, InternUserValidator>();
        builder.Services.AddScoped<IProjectMaintenanceValidator, ProjectMaintenanceValidator>();

        // Register Services
        builder.Services.AddScoped<IStateService, StateService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        builder.Services.AddScoped<IEmployeeInfoService, EmployeeInfoService>();
        builder.Services.AddScoped<IInternUserService, InternUserService>();
        builder.Services.AddScoped<IProjectMaintenanceService, ProjectMaintenanceService>();

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
