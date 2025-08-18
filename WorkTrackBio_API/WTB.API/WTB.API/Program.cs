using WTB.API.Data.Context;
using WTB.API.Data.Configurations;
using WTB.API.Extensions;
using WTB.API.Data.Interceptors;
using WTB.API.Models.Configuration;
using WTB.API.Services;
using WTB.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar HttpContextAccessor para el interceptor de auditoría
builder.Services.AddHttpContextAccessor();

// Configurar base de datos con interceptor de auditoría
builder.Services.AddDatabaseServices(builder.Configuration);

// Configurar repositorios
builder.Services.AddRepositoryServices();

// Configurar servicios de negocio
builder.Services.AddBusinessServices();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configurar FluentValidation
builder.Services.AddFluentValidationServices();

// Configurar opciones del sistema
builder.Services.Configure<SystemSettings>(
    builder.Configuration.GetSection("SystemSettings"));

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "WorkTrackBio API", 
        Version = "v1",
        Description = "API para gestión de tiempo y proyectos con soporte biométrico" 
    });
    
    // Incluir comentarios XML para documentación
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Aplicar migraciones automáticamente en desarrollo
    // await app.UseDatabaseMigrationAsync(); // Comentado temporalmente
}

// Agregar middleware de manejo global de errores
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();