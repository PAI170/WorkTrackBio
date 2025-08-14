using FluentValidation;
using FluentValidation.AspNetCore;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Models.DTOs.Project;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Models.DTOs.Auth;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Validators.Employee;
using WTB.API.Validators.Project;
using WTB.API.Validators.Assistance;
using WTB.API.Validators.InternUser;
using WTB.API.Validators.Auth;
using WTB.API.Validators.ProjectAssign;

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

            // Registrar validadores de Project
            services.AddScoped<IValidator<CreateProjectDto>, CreateProjectValidator>();
            services.AddScoped<IValidator<UpdateProjectDto>, UpdateProjectValidator>();

            // Registrar validadores de Assistance
            services.AddScoped<IValidator<CheckInDto>, CheckInValidator>();
            services.AddScoped<IValidator<CheckOutDto>, CheckOutValidator>();
            services.AddScoped<IValidator<UpdateAssistanceDto>, UpdateAssistanceValidator>();

            // Registrar validadores de InternUser
            services.AddScoped<IValidator<CreateInternUserDto>, CreateInternUserValidator>();
            services.AddScoped<IValidator<UpdateInternUserDto>, UpdateInternUserValidator>();
            services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidator>();

            // Registrar validadores de Auth
            services.AddScoped<IValidator<LoginDto>, LoginValidator>();

            // Registrar validadores de ProjectAssign
            services.AddScoped<IValidator<CreateProjectAssignDto>, CreateProjectAssignValidator>();
            services.AddScoped<IValidator<UpdateProjectAssignDto>, UpdateProjectAssignValidator>();
            services.AddScoped<IValidator<ProjectAssignFilterDto>, ProjectAssignFilterValidator>();

            return services;
        }
    }
}
