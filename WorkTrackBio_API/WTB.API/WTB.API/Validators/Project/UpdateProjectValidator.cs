using FluentValidation;
using WTB.API.Models.DTOs.Project;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.Project
{
    /// <summary>
    /// Validador para la actualización de proyectos
    /// </summary>
    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectValidator()
        {
            // Validación del ID
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del proyecto debe ser mayor que cero");

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

            // Validación de fecha de inicio (más flexible para updates)
            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today.AddYears(-2))
                .WithMessage("La fecha de inicio no puede ser anterior a 2 años")
                .When(x => x.StartDate.HasValue);

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

            // Validación específica para proyectos completados
            RuleFor(x => x.EndDate)
                .Must(BeInThePast)
                .WithMessage("Un proyecto completado debe tener una fecha de fin en el pasado")
                .When(x => x.StateId == 5); // Estado "Completed"
        }

        /// <summary>
        /// Valida que el nombre del proyecto tenga caracteres válidos
        /// </summary>
        private bool BeValidProjectName(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return false;

            var projectNamePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9\s\-\.\,\(\)\&\+]+$";
            return Regex.IsMatch(projectName, projectNamePattern);
        }

        /// <summary>
        /// Valida que el rango de fechas sea lógico
        /// </summary>
        private bool BeValidDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return true;

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
            var validProjectStates = new[] { 1, 2, 4, 5, 6 };
            return validProjectStates.Contains(stateId);
        }

        /// <summary>
        /// Valida que la fecha esté en el pasado
        /// </summary>
        private bool BeInThePast(DateTime? date)
        {
            if (!date.HasValue)
                return false;

            return date.Value.Date <= DateTime.Today;
        }
    }
}

