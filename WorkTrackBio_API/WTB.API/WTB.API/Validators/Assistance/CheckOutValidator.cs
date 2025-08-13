using FluentValidation;
using WTB.API.Models.DTOs.Assistance;

namespace WTB.API.Validators.Assistance
{
    /// <summary>
    /// Validador para el registro de check-out
    /// </summary>
    public class CheckOutValidator : AbstractValidator<CheckOutDto>
    {
        public CheckOutValidator()
        {
            // Validación del ID de asistencia
            RuleFor(x => x.AssistanceId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un registro de asistencia válido");

            // Validación de fecha y hora de check-out
            RuleFor(x => x.CheckOut)
                .NotEmpty()
                .WithMessage("La fecha y hora de check-out es obligatoria")
                .Must(BeValidCheckOutTime)
                .WithMessage("La hora de check-out debe estar dentro del horario laboral (5:00 AM - 11:59 PM)")
                .Must(BeValidCheckOutDate)
                .WithMessage("El check-out no puede ser en el futuro ni más de 1 día en el pasado");

            // Validación de notas (opcional pero con formato válido)
            RuleFor(x => x.Notes)
                .Must(BeValidNotes)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Las notas solo pueden contener caracteres alfanuméricos, espacios y signos de puntuación básicos");

            // TODO: Validación que requiere acceso a datos - CheckOut > CheckIn
            // Esta validación se implementará en el servicio ya que requiere consultar el CheckIn original
        }

        /// <summary>
        /// Valida que la hora de check-out esté dentro del horario laboral extendido
        /// </summary>
        private bool BeValidCheckOutTime(DateTime checkOut)
        {
            var timeOfDay = checkOut.TimeOfDay;
            var minTime = new TimeSpan(5, 0, 0);  // 5:00 AM
            var maxTime = new TimeSpan(23, 59, 59); // 11:59 PM (permite checkout tardío)
            
            return timeOfDay >= minTime && timeOfDay <= maxTime;
        }

        /// <summary>
        /// Valida que la fecha de check-out sea lógica
        /// </summary>
        private bool BeValidCheckOutDate(DateTime checkOut)
        {
            var now = DateTime.Now;
            var minDate = now.AddDays(-1); // Máximo 1 día en el pasado para check-out
            
            return checkOut >= minDate && checkOut <= now.AddMinutes(5); // 5 minutos de tolerancia
        }

        /// <summary>
        /// Valida que las notas tengan un formato apropiado
        /// </summary>
        private bool BeValidNotes(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return true;

            var forbiddenChars = new[] { '<', '>', '"', '\'', '&', '\0', '\r', '\n' };
            return !forbiddenChars.Any(c => notes.Contains(c));
        }
    }
}




