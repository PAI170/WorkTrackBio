using FluentValidation;
using WTB.API.Models.DTOs.Project;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.Project
{
    /// <summary>
    /// Validador para la creación de proyectos con reglas de negocio específicas
    /// </summary>
    public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectValidator()
        {
            // Validación del nombre del proyecto
            RuleFor(x => x.ProjectName)
                .NotEmpty()
                .WithMessage("El nombre del proyecto es obligatorio")
                .Must(BeValidProjectName)
                .WithMessage("El nombre del proyecto solo puede contener letras, números, espacios y algunos caracteres especiales");

            // Validación de fechas lógicas
            RuleFor(x => x)
                .Must(x => BeValidDateRange(x.StartDate, x.EndDate))
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithName("EndDate");

            // Validación de fecha de inicio
            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30))
                .WithMessage("La fecha de inicio no puede ser anterior a 30 días desde hoy")
                .When(x => x.StartDate.HasValue);

            // Validación de fecha de fin
            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.Today)
                .WithMessage("La fecha de fin debe ser futura")
                .When(x => x.EndDate.HasValue);

            // Validación de duración del proyecto
            RuleFor(x => x)
                .Must(x => BeValidProjectDuration(x.StartDate, x.EndDate))
                .WithMessage("La duración del proyecto no puede exceder 5 años")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithName("EndDate");

            // Validación del estado del proyecto
            RuleFor(x => x.StateId)
                .Must(BeValidProjectState)
                .WithMessage("El estado seleccionado no es válido para proyectos");
        }

        /// <summary>
        /// Valida que el nombre del proyecto tenga caracteres válidos
        /// </summary>
        private bool BeValidProjectName(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return false;

            // Permite letras, números, espacios, guiones, puntos, comas y algunos caracteres especiales
            var projectNamePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9\s\-\.\,\(\)\&\+]+$";
            return Regex.IsMatch(projectName, projectNamePattern);
        }

        /// <summary>
        /// Valida que el rango de fechas sea lógico
        /// </summary>
        private bool BeValidDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return true; // Las fechas son opcionales

            return endDate.Value.Date >= startDate.Value.Date;
        }

        /// <summary>
        /// Valida que la duración del proyecto no sea excesiva
        /// </summary>
        private bool BeValidProjectDuration(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return true;

            var duration = endDate.Value - startDate.Value;
            return duration.TotalDays <= (365 * 5); // Máximo 5 años
        }

        /// <summary>
        /// Valida que el estado sea apropiado para proyectos
        /// </summary>
        private bool BeValidProjectState(int stateId)
        {
            // Estados válidos para proyectos: 4=In Progress, 5=Completed, 6=On Hold
            // También podemos aceptar estados generales: 1=Active, 2=Inactive
            var validProjectStates = new[] { 1, 2, 4, 5, 6 };
            return validProjectStates.Contains(stateId);
        }
    }
}
