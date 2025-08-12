using WTB.API.Data.Context;
using WTB.API.Data.Configurations;
using WTB.API.Extensions;
using WTB.API.Data.Interceptors;
using WTB.API.Models.Configuration;
using WTB.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar HttpContextAccessor para el interceptor de auditoría
builder.Services.AddHttpContextAccessor();

// Configurar base de datos con interceptor de auditoría
builder.Services.AddDatabaseServices(builder.Configuration);

// Configurar FluentValidation
builder.Services.AddFluentValidationServices();

// Configurar opciones del sistema
builder.Services.Configure<SystemSettings>(
    builder.Configuration.GetSection("SystemSettings"));

// Registrar servicios de control de acceso
builder.Services.AddScoped<IDeviceAccessControlService, DeviceAccessControlService>();

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
    await app.UseDatabaseMigrationAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();