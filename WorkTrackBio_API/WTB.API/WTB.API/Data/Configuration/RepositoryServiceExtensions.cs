using Microsoft.Extensions.DependencyInjection;
using WTB.API.Data.Repositories;

namespace WTB.API.Data.Configurations
{
    /// <summary>
    /// Extensiones para configurar los servicios de repositorios
    /// </summary>
    public static class RepositoryServiceExtensions
    {
        /// <summary>
        /// Agrega los servicios de repositorios al contenedor de DI
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios configurada</returns>
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Registrar Unit of Work como scoped (por request)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registrar repositorios específicos como scoped
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAssistanceRepository, AssistanceRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();

            // Los repositorios genéricos se crean a través del Unit of Work
            // No es necesario registrarlos individualmente

            return services;
        }
    }
}
