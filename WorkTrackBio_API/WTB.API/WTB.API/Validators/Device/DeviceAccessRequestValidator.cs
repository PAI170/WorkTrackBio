using FluentValidation;
using WTB.API.Models.DTOs.Device;

namespace WTB.API.Validators.Device
{
    /// <summary>
    /// Validador para DeviceAccessRequestDto
    /// </summary>
    public class DeviceAccessRequestValidator : AbstractValidator<DeviceAccessRequestDto>
    {
        public DeviceAccessRequestValidator()
        {
            // Validación de DeviceId
            RuleFor(x => x.DeviceId)
                .GreaterThan(0)
                .WithMessage("El ID del dispositivo debe ser un número válido mayor a 0");

            // Validación de EmployeeId
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser un número válido mayor a 0");

            // Validación de AccessType
            RuleFor(x => x.AccessType)
                .NotEmpty()
                .WithMessage("El tipo de acceso es requerido")
                .Must(BeValidAccessType)
                .WithMessage("El tipo de acceso debe ser CHECKIN o CHECKOUT");

            // Validación de Notes (opcional)
            When(x => !string.IsNullOrEmpty(x.Notes), () =>
            {
                RuleFor(x => x.Notes)
                    .MaximumLength(500)
                    .WithMessage("Las notas no pueden exceder 500 caracteres")
                    .Must(BeValidNotes)
                    .WithMessage("Las notas contienen caracteres peligrosos");
            });
        }

        /// <summary>
        /// Valida que el tipo de acceso sea válido
        /// </summary>
        private static bool BeValidAccessType(string accessType)
        {
            var validTypes = new[] { "CHECKIN", "CHECKOUT" };
            return validTypes.Contains(accessType, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida que las notas no contengan caracteres peligrosos
        /// </summary>
        private static bool BeValidNotes(string notes)
        {
            var dangerousChars = new[] { "<", ">", "script", "javascript", "onload", "onerror" };
            
            return !dangerousChars.Any(dc => 
                notes.Contains(dc, StringComparison.OrdinalIgnoreCase));
        }
    }
}
