using FluentValidation;
using FluentValidation.AspNetCore;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Validators.Employee;

namespace WTB.API.Extensions
{
    public static class FluentValidationServiceExtensions
    {
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            // Configurar FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            // Registrar validadores de Employee
            services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeValidator>();
            services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();

            return services;
        }
    }
}
