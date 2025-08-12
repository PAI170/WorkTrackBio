using FluentValidation;
using WTB.API.Models.DTOs.ProjectWarranty;

namespace WTB.API.Validators.ProjectWarranty
{
    /// <summary>
    /// Validador para CreateProjectWarrantyDto
    /// </summary>
    public class CreateProjectWarrantyValidator : AbstractValidator<CreateProjectWarrantyDto>
    {
        public CreateProjectWarrantyValidator()
        {
            // Validación de IdProject
            RuleFor(x => x.IdProject)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un proyecto válido");

            // Validación de WarrantyDescription
            RuleFor(x => x.WarrantyDescription)
                .NotEmpty()
                .WithMessage("La descripción de la garantía es requerida")
                .MaximumLength(1000)
                .WithMessage("La descripción de la garantía no puede exceder 1000 caracteres")
                .Must(BeValidDescription)
                .WithMessage("La descripción de la garantía contiene caracteres no permitidos");

            // Validación de MadeById
            RuleFor(x => x.MadeById)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido");

            // Validación de WarrantyCost (opcional)
            When(x => x.WarrantyCost.HasValue, () =>
            {
                RuleFor(x => x.WarrantyCost!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("El costo de la garantía no puede ser negativo")
                    .LessThanOrEqualTo(999999.99m)
                    .WithMessage("El costo de la garantía no puede exceder 999,999.99");
            });

            // Validación de StateId
            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un estado válido");

            // Validación de WarrantyDate (opcional)
            When(x => x.WarrantyDate.HasValue, () =>
            {
                RuleFor(x => x.WarrantyDate!.Value)
                    .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                    .WithMessage("La fecha de garantía no puede ser futura (máximo 1 día)")
                    .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-10))
                    .WithMessage("La fecha de garantía no puede ser anterior a 10 años");
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
    }
}
