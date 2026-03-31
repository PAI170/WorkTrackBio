using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Interfaces;
using WorkTrackBio.API.Services;

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

//SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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
