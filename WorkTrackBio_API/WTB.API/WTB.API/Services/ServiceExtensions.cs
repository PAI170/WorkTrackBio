using Microsoft.Extensions.DependencyInjection;
using WTB.API.Services.Interfaces;
using WTB.API.Services.Implementations;

namespace WTB.API.Services
{
    /// <summary>
    /// Extensiones para configurar y registrar los servicios de negocio
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registra todos los servicios de negocio en el contenedor de DI
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios configurada</returns>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Registrar servicios de negocio principales
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAssistanceService, AssistanceService>();
            services.AddScoped<IProjectService, ProjectService>();

            // Los servicios existentes ya están registrados en Program.cs
            // BiometricAttendanceService y DeviceAccessControlService

            return services;
        }
    }
}
