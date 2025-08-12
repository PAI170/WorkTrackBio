using FluentValidation;
using WTB.API.Models.DTOs.AuditRegister;

namespace WTB.API.Validators.AuditRegister
{
    /// <summary>
    /// Validador para CreateAuditRegisterDto
    /// </summary>
    public class CreateAuditRegisterValidator : AbstractValidator<CreateAuditRegisterDto>
    {
        public CreateAuditRegisterValidator()
        {
            // Validación de AssistanceId
            RuleFor(x => x.AssistanceId)
                .GreaterThan(0)
                .WithMessage("El ID de asistencia debe ser un número válido mayor a 0");

            // Validación de ActionType
            RuleFor(x => x.ActionType)
                .NotEmpty()
                .WithMessage("El tipo de acción es requerido")
                .MaximumLength(50)
                .WithMessage("El tipo de acción no puede exceder 50 caracteres")
                .Must(BeValidActionType)
                .WithMessage("El tipo de acción debe ser uno de los valores permitidos");

            // Validación de DetailChange
            RuleFor(x => x.DetailChange)
                .NotEmpty()
                .WithMessage("El detalle del cambio es requerido")
                .MaximumLength(1000)
                .WithMessage("El detalle del cambio no puede exceder 1000 caracteres")
                .Must(BeValidDetailChange)
                .WithMessage("El detalle del cambio contiene caracteres no permitidos");

            // Validación de AdminId
            RuleFor(x => x.AdminId)
                .GreaterThan(0)
                .WithMessage("El ID del administrador debe ser un número válido mayor a 0");

            // Validación de ActionDate (si se proporciona)
            When(x => x.ActionDate.HasValue, () =>
            {
                RuleFor(x => x.ActionDate!.Value)
                    .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                    .WithMessage("La fecha de acción no puede ser futura (máximo 5 minutos de diferencia)");
            });
        }

        /// <summary>
        /// Valida que el tipo de acción sea uno de los permitidos
        /// </summary>
        private static bool BeValidActionType(string actionType)
        {
            var validTypes = new[]
            {
                "TIME_CORRECTION",
                "MANUAL_ENTRY", 
                "ADMIN_OVERRIDE",
                "DATA_FIX",
                "SYSTEM_ADJUSTMENT",
                "EMPLOYEE_UPDATE",
                "PROJECT_UPDATE",
                "DEVICE_UPDATE"
            };

            return validTypes.Contains(actionType.ToUpper());
        }

        /// <summary>
        /// Valida que el detalle del cambio no contenga caracteres peligrosos
        /// </summary>
        private static bool BeValidDetailChange(string detailChange)
        {
            // Caracteres peligrosos que no deben estar en el detalle
            var dangerousChars = new[] { "<", ">", "script", "javascript", "onload", "onerror" };
            
            return !dangerousChars.Any(dc => 
                detailChange.Contains(dc, StringComparison.OrdinalIgnoreCase));
        }
    }
}
