using FluentValidation;
using WTB.API.Models.DTOs.Assistance;

namespace WTB.API.Validators.Assistance
{
    /// <summary>
    /// Validador para el registro de check-in
    /// </summary>
    public class CheckInValidator : AbstractValidator<CheckInDto>
    {
        public CheckInValidator()
        {
            // Validación de empleado
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido");

            // Validación de proyecto
            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un proyecto válido");

            // Validación de fecha y hora de check-in
            RuleFor(x => x.CheckIn)
                .NotEmpty()
                .WithMessage("La fecha y hora de check-in es obligatoria")
                .Must(BeValidCheckInTime)
                .WithMessage("La hora de check-in debe estar dentro del horario laboral (5:00 AM - 11:00 PM)")
                .Must(BeValidCheckInDate)
                .WithMessage("El check-in no puede ser en el futuro ni más de 7 días en el pasado")
                .Must(BeReasonableCheckInTime)
                .WithMessage("El check-in debe estar cerca de la hora actual (máximo 30 minutos de diferencia para registros en tiempo real)");

            // Validación de notas (opcional pero con formato válido)
            RuleFor(x => x.Notes)
                .Must(BeValidNotes)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Las notas solo pueden contener caracteres alfanuméricos, espacios y signos de puntuación básicos");

            // TODO: Validaciones que requieren acceso a datos:
            // - El empleado no debe tener un check-in activo sin check-out
            // - El empleado debe estar asignado al proyecto
            // - El proyecto debe estar activo
            // Estas validaciones se implementarán en el servicio
        }

        /// <summary>
        /// Valida que la hora de check-in esté dentro del horario laboral
        /// </summary>
        private bool BeValidCheckInTime(DateTime checkIn)
        {
            var timeOfDay = checkIn.TimeOfDay;
            var minTime = new TimeSpan(5, 0, 0);  // 5:00 AM
            var maxTime = new TimeSpan(23, 0, 0); // 11:00 PM
            
            return timeOfDay >= minTime && timeOfDay <= maxTime;
        }

        /// <summary>
        /// Valida que la fecha de check-in sea lógica
        /// </summary>
        private bool BeValidCheckInDate(DateTime checkIn)
        {
            var now = DateTime.Now;
            var minDate = now.AddDays(-7); // Máximo 7 días en el pasado (para correcciones)
            var maxDate = now.AddMinutes(5); // 5 minutos de tolerancia en el futuro
            
            return checkIn >= minDate && checkIn <= maxDate;
        }

        /// <summary>
        /// Valida que el check-in sea razonable para registros en tiempo real
        /// </summary>
        private bool BeReasonableCheckInTime(DateTime checkIn)
        {
            var now = DateTime.Now;
            var timeDifference = Math.Abs((now - checkIn).TotalMinutes);
            
            // Para check-ins recientes (menos de 1 hora), debe estar cerca del tiempo actual
            if (timeDifference <= 60)
            {
                return timeDifference <= 30; // Máximo 30 minutos de diferencia
            }
            
            // Para check-ins más antiguos (correcciones), esta validación no aplica
            return true;
        }

        /// <summary>
        /// Valida que las notas tengan un formato apropiado
        /// </summary>
        private bool BeValidNotes(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return true;

            // Evitar caracteres especiales que podrían ser problemáticos
            var forbiddenChars = new[] { '<', '>', '"', '\'', '&', '\0', '\r', '\n' };
            return !forbiddenChars.Any(c => notes.Contains(c));
        }
    }
}




