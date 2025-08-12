using FluentValidation;
using WTB.API.Models.DTOs.FingerPrint;

namespace WTB.API.Validators.FingerPrint
{
    /// <summary>
    /// Validador para CreateFingerPrintDto
    /// </summary>
    public class CreateFingerPrintValidator : AbstractValidator<CreateFingerPrintDto>
    {
        public CreateFingerPrintValidator()
        {
            // Validación de EmployeeId
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser un número válido mayor a 0");

            // Validación de TemplateFingerPrint
            RuleFor(x => x.TemplateFingerPrint)
                .NotNull()
                .WithMessage("El template de huella es requerido")
                .NotEmpty()
                .WithMessage("El template de huella no puede estar vacío")
                .Must(BeValidTemplateSize)
                .WithMessage("El template de huella debe tener un tamaño válido (entre 100 y 10000 bytes)")
                .Must(BeValidTemplateFormat)
                .WithMessage("El template de huella tiene un formato inválido");

            // Validación de IssueDate (opcional)
            When(x => x.IssueDate.HasValue, () =>
            {
                RuleFor(x => x.IssueDate!.Value)
                    .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                    .WithMessage("La fecha de emisión no puede ser futura (máximo 5 minutos de diferencia)")
                    .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-10))
                    .WithMessage("La fecha de emisión no puede ser anterior a 10 años");
            });
        }

        /// <summary>
        /// Valida que el tamaño del template sea razonable
        /// </summary>
        private static bool BeValidTemplateSize(byte[] template)
        {
            if (template == null) return false;
            
            // Los templates de huella típicamente están entre 100 y 10000 bytes
            return template.Length >= 100 && template.Length <= 10000;
        }

        /// <summary>
        /// Valida que el template tenga un formato válido
        /// </summary>
        private static bool BeValidTemplateFormat(byte[] template)
        {
            if (template == null || template.Length == 0) return false;

            // Verificar que no sea todo ceros o todo unos (templates inválidos)
            var allZeros = template.All(b => b == 0);
            var allOnes = template.All(b => b == 255);
            
            if (allZeros || allOnes)
            {
                return false;
            }

            // Verificar que tenga cierta variabilidad (no sea un patrón repetitivo)
            var firstByte = template[0];
            var repetitivePattern = template.Take(10).All(b => b == firstByte);
            
            if (repetitivePattern)
            {
                return false;
            }

            return true;
        }
    }
}
