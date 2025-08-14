using FluentValidation;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.Enums;

namespace WTB.API.Validators.Assistance
{
    /// <summary>
    /// Validador para la actualización de registros de asistencia (solo administradores)
    /// </summary>
    public class UpdateAssistanceValidator : AbstractValidator<UpdateAssistanceDto>
    {
        public UpdateAssistanceValidator()
        {
            // Validación del ID
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del registro de asistencia debe ser mayor que cero");

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
                .Must(BeValidCheckInDate)
                .WithMessage("El check-in no puede ser más de 30 días en el pasado ni en el futuro");

            // Validación de check-out (si está presente)
            RuleFor(x => x.CheckOut)
                .Must(BeValidCheckOutDate)
                .When(x => x.CheckOut.HasValue)
                .WithMessage("El check-out no puede ser más de 30 días en el pasado ni en el futuro");

            // Validación de lógica temporal (CheckOut > CheckIn)
            RuleFor(x => x)
                .Must(x => BeValidTimeRange(x.CheckIn, x.CheckOut))
                .WithMessage("La hora de check-out debe ser posterior a la hora de check-in")
                .When(x => x.CheckOut.HasValue)
                .WithName("CheckOut");

            // Validación de horas totales
            RuleFor(x => x.TotalHours)
                .GreaterThan(0)
                .WithMessage("Las horas totales deben ser mayor que cero")
                .LessThanOrEqualTo(24)
                .WithMessage("Las horas totales no pueden exceder 24 horas en un día")
                .When(x => x.TotalHours.HasValue);

            // Validación de consistencia entre horas calculadas y horas reportadas
            RuleFor(x => x)
                .Must(x => BeConsistentHours(x.CheckIn, x.CheckOut, x.TotalHours))
                .WithMessage("Las horas totales no coinciden con la diferencia entre check-in y check-out")
                .When(x => x.CheckOut.HasValue && x.TotalHours.HasValue)
                .WithName("TotalHours");

            // Validación del tipo de registro
            RuleFor(x => x.RegisterType)
                .NotEmpty()
                .WithMessage("El tipo de registro es obligatorio")
                .Must(BeValidRegisterType)
                .WithMessage("El tipo de registro debe ser válido (CheckIn, CheckOut, Manual)");

            // Validación de notas
            RuleFor(x => x.Notes)
                .Must(BeValidNotes)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Las notas solo pueden contener caracteres alfanuméricos, espacios y signos de puntuación básicos");
        }

        /// <summary>
        /// Valida que la fecha de check-in sea razonable para updates administrativos
        /// </summary>
        private bool BeValidCheckInDate(DateTime checkIn)
        {
            var now = DateTime.Now;
            var minDate = now.AddDays(-30); // Máximo 30 días en el pasado para updates
            var maxDate = now.AddMinutes(5); // 5 minutos de tolerancia en el futuro
            
            return checkIn >= minDate && checkIn <= maxDate;
        }

        /// <summary>
        /// Valida que la fecha de check-out sea razonable
        /// </summary>
        private bool BeValidCheckOutDate(DateTime? checkOut)
        {
            if (!checkOut.HasValue)
                return true;

            var now = DateTime.Now;
            var minDate = now.AddDays(-30);
            var maxDate = now.AddMinutes(5);
            
            return checkOut.Value >= minDate && checkOut.Value <= maxDate;
        }

        /// <summary>
        /// Valida que el rango de tiempo sea lógico
        /// </summary>
        private bool BeValidTimeRange(DateTime checkIn, DateTime? checkOut)
        {
            if (!checkOut.HasValue)
                return true;

            return checkOut.Value > checkIn;
        }

        /// <summary>
        /// Valida que las horas totales sean consistentes con los tiempos de check-in/out
        /// </summary>
        private bool BeConsistentHours(DateTime checkIn, DateTime? checkOut, decimal? totalHours)
        {
            if (!checkOut.HasValue || !totalHours.HasValue)
                return true;

            var calculatedHours = (decimal)(checkOut.Value - checkIn).TotalHours;
            var tolerance = 0.25m; // 15 minutos de tolerancia
            
            return Math.Abs(calculatedHours - totalHours.Value) <= tolerance;
        }

        /// <summary>
        /// Valida que el tipo de registro sea válido
        /// </summary>
        private bool BeValidRegisterType(string registerType)
        {
            return Enum.TryParse<RegisterTypeEnum>(registerType, true, out _);
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






