using Microsoft.Extensions.DependencyInjection;
using WTB.API.Services.Interfaces;
using WTB.API.Services.Implementations;
using WTB.API.Services;
using WTB.API.Data.Repositories;

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
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IInternUserService, InternUserService>();
            services.AddScoped<IProjectMaintenanceService, ProjectMaintenanceService>();
            services.AddScoped<IProjectWarrantyService, ProjectWarrantyService>();
            services.AddScoped<IProjectAssignService, ProjectAssignService>();
            
            // Registrar servicios de control de acceso y biométricos
            services.AddScoped<IBiometricAttendanceService, BiometricAttendanceService>();
            services.AddScoped<IDeviceAccessControlService, DeviceAccessControlService>();

            // Registrar repositorios
            services.AddScoped<IInternUserRepository, InternUserRepository>();
            services.AddScoped<IProjectMaintenanceRepository, ProjectMaintenanceRepository>();
            services.AddScoped<IProjectWarrantyRepository, ProjectWarrantyRepository>();

            return services;
        }
    }
}
