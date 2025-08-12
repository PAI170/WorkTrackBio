using FluentValidation;
using WTB.API.Models.DTOs.ProjectMaintenance;

namespace WTB.API.Validators.ProjectMaintenance
{
    /// <summary>
    /// Validador para CreateProjectMaintenanceDto
    /// </summary>
    public class CreateProjectMaintenanceValidator : AbstractValidator<CreateProjectMaintenanceDto>
    {
        public CreateProjectMaintenanceValidator()
        {
            // Validación de IdProject
            RuleFor(x => x.IdProject)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un proyecto válido");

            // Validación de MaintenanceDescription
            RuleFor(x => x.MaintenanceDescription)
                .NotEmpty()
                .WithMessage("La descripción del mantenimiento es requerida")
                .MaximumLength(1000)
                .WithMessage("La descripción del mantenimiento no puede exceder 1000 caracteres")
                .Must(BeValidDescription)
                .WithMessage("La descripción del mantenimiento contiene caracteres no permitidos");

            // Validación de MadeById
            RuleFor(x => x.MadeById)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido");

            // Validación de MaintenanceCost (opcional)
            When(x => x.MaintenanceCost.HasValue, () =>
            {
                RuleFor(x => x.MaintenanceCost!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("El costo del mantenimiento no puede ser negativo")
                    .LessThanOrEqualTo(999999.99m)
                    .WithMessage("El costo del mantenimiento no puede exceder 999,999.99");
            });

            // Validación de AdditionalInfo (opcional)
            When(x => !string.IsNullOrEmpty(x.AdditionalInfo), () =>
            {
                RuleFor(x => x.AdditionalInfo)
                    .MaximumLength(255)
                    .WithMessage("La información adicional no puede exceder 255 caracteres")
                    .Must(BeValidAdditionalInfo)
                    .WithMessage("La información adicional contiene caracteres peligrosos");
            });

            // Validación de StateId
            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un estado válido");

            // Validación de MaintenanceDate (opcional)
            When(x => x.MaintenanceDate.HasValue, () =>
            {
                RuleFor(x => x.MaintenanceDate!.Value)
                    .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                    .WithMessage("La fecha de mantenimiento no puede ser futura (máximo 1 día)")
                    .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-5))
                    .WithMessage("La fecha de mantenimiento no puede ser anterior a 5 años");
            });
        }

        /// <summary>
        /// Valida que la descripción no contenga caracteres peligrosos
        /// </summary>
        private static bool BeValidDescription(string description)
        {
            var dangerousChars = new[] { "<", ">", "script", "javascript", "onload", "onerror" };
            
            return !dangerousChars.Any(dc => 
                description.Contains(dc, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Valida que la información adicional no contenga caracteres peligrosos
        /// </summary>
        private static bool BeValidAdditionalInfo(string additionalInfo)
        {
            var dangerousChars = new[] { "<", ">", "script", "javascript", "onload", "onerror" };
            
            return !dangerousChars.Any(dc => 
                additionalInfo.Contains(dc, StringComparison.OrdinalIgnoreCase));
        }
    }
}
