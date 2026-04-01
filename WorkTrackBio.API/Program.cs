using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Interfaces;
using WorkTrackBio.API.Services;
using WorkTrackBio.API.Validators.State;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value!.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = ApiResponse<object>.ErrorResponse("Error en los datos enviados", 400, errors);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(
               CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<StateCreateValidator>();

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

//SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IStateService, StateService>();
//-----------------------------------------------------------

//MAPSTER
var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();
//------------------------------------------------------------
var app = builder.Build();

//GlobalMiddleWare
app.UseMiddleware<WorkTrackBio.API.Middlewares.GlobalExceptionMiddleware>();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.ContentType = "application/json";
        var response = ApiResponse<object>.ErrorResponse("La ruta o el recurso solicitado no existe.", 404);
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase };
        var json = System.Text.Json.JsonSerializer.Serialize(response, jsonOptions);
        await context.HttpContext.Response.WriteAsync(json);
    }
});
//-------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});
}

app.UseCors("DevelopmentPolicy");

app.MapControllers();

app.Run();
