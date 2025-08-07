using Microsoft.EntityFrameworkCore;
using WTB.API.Data.Context;

namespace WTB.API.Data.Configurations
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar Entity Framework con SQL Server
            services.AddDbContext<WTBDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });

                // Solo en desarrollo - habilita logging sensible
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            return services;
        }

        public static async Task<IApplicationBuilder> UseDatabaseMigrationAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WTBDbContext>();
            
            try
            {
                // Aplicar migraciones pendientes automáticamente
                await context.Database.MigrateAsync();
                Console.WriteLine("✅ Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database migration failed: {ex.Message}");
                throw;
            }

            return app;
        }
    }
}