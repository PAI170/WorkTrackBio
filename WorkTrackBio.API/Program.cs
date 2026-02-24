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
using WorkTrackBio.API.Repositories.AssistanceRepository;
using WorkTrackBio.API.Services.AssistanceService;
using WorkTrackBio.API.Validators.AssistanceValidator;
using WorkTrackBio.API.Repositories.ProjectWarrantyRepository;
using WorkTrackBio.API.Services.ProjectWarrantyService;
using WorkTrackBio.API.Validators.ProjectWarrantyValidator;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddDbContext<WorkTrackBioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:3000",
                "http://localhost:8080"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

        builder.Services.AddScoped<IStateRepository, StateRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        builder.Services.AddScoped<IEmployeeInfoRepository, EmployeeInfoRepository>();
        builder.Services.AddScoped<IInternUserRepository, InternUserRepository>();
        builder.Services.AddScoped<IProjectMaintenanceRepository, ProjectMaintenanceRepository>();
        builder.Services.AddScoped<IAssistanceRepository, AssistanceRepository>();
        builder.Services.AddScoped<IProjectWarrantyRepository, ProjectWarrantyRepository>();

        builder.Services.AddScoped<IStateValidator, StateValidator>();
        builder.Services.AddScoped<IRoleValidator, RoleValidator>();
        builder.Services.AddScoped<IProjectValidator, ProjectValidator>();
        builder.Services.AddScoped<IDocumentTypeValidator, DocumentTypeValidator>();
        builder.Services.AddScoped<IEmployeeInfoValidator, EmployeeInfoValidator>();
        builder.Services.AddScoped<IDocumentValidator, DocumentValidator>();
        builder.Services.AddScoped<IPhoneNumberFormatter, PhoneNumberFormatter>();
        builder.Services.AddScoped<IInternUserValidator, InternUserValidator>();
        builder.Services.AddScoped<IProjectMaintenanceValidator, ProjectMaintenanceValidator>();
        builder.Services.AddScoped<IAssistanceValidator, AssistanceValidator>();
        builder.Services.AddScoped<IProjectWarrantyValidator, ProjectWarrantyValidator>();

        builder.Services.AddScoped<IStateService, StateService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        builder.Services.AddScoped<IEmployeeInfoService, EmployeeInfoService>();
        builder.Services.AddScoped<IInternUserService, InternUserService>();
        builder.Services.AddScoped<IProjectMaintenanceService, ProjectMaintenanceService>();
        builder.Services.AddScoped<IAssistanceService, AssistanceService>();
        builder.Services.AddScoped<IProjectWarrantyService, ProjectWarrantyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevelopmentPolicy");

app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});

app.MapControllers();

app.Run();
