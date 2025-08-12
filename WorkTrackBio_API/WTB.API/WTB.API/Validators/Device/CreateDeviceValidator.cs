using FluentValidation;
using WTB.API.Models.DTOs.Device;

namespace WTB.API.Validators.Device
{
    /// <summary>
    /// Validador para CreateDeviceDto
    /// </summary>
    public class CreateDeviceValidator : AbstractValidator<CreateDeviceDto>
    {
        public CreateDeviceValidator()
        {
            // Validación de DeviceId
            RuleFor(x => x.DeviceId)
                .NotEmpty()
                .WithMessage("El ID del dispositivo es requerido")
                .MaximumLength(50)
                .WithMessage("El ID del dispositivo no puede exceder 50 caracteres")
                .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithMessage("El ID del dispositivo solo puede contener letras, números, guiones y guiones bajos");

            // Validación de DeviceName
            RuleFor(x => x.DeviceName)
                .NotEmpty()
                .WithMessage("El nombre del dispositivo es requerido")
                .MaximumLength(100)
                .WithMessage("El nombre del dispositivo no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-Z0-9\s\-_\.]+$")
                .WithMessage("El nombre del dispositivo contiene caracteres no permitidos");

            // Validación de ProjectId
            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un proyecto válido");

            // Validación de Location (opcional)
            When(x => !string.IsNullOrEmpty(x.Location), () =>
            {
                RuleFor(x => x.Location)
                    .MaximumLength(200)
                    .WithMessage("La ubicación no puede exceder 200 caracteres")
                    .Matches(@"^[a-zA-Z0-9\s\-_\.\,\#]+$")
                    .WithMessage("La ubicación contiene caracteres no permitidos");
            });

            // Validación de DeviceType
            When(x => !string.IsNullOrEmpty(x.DeviceType), () =>
            {
                RuleFor(x => x.DeviceType)
                    .MaximumLength(50)
                    .WithMessage("El tipo de dispositivo no puede exceder 50 caracteres")
                    .Must(BeValidDeviceType)
                    .WithMessage("El tipo de dispositivo debe ser uno de los valores permitidos");
            });

            // Validación de IpAddress (opcional)
            When(x => !string.IsNullOrEmpty(x.IpAddress), () =>
            {
                RuleFor(x => x.IpAddress)
                    .MaximumLength(100)
                    .WithMessage("La dirección IP no puede exceder 100 caracteres")
                    .Must(BeValidIpAddress)
                    .WithMessage("La dirección IP tiene un formato inválido");
            });

            // Validación de SerialNumber (opcional)
            When(x => !string.IsNullOrEmpty(x.SerialNumber), () =>
            {
                RuleFor(x => x.SerialNumber)
                    .MaximumLength(20)
                    .WithMessage("El número de serie no puede exceder 20 caracteres")
                    .Matches(@"^[a-zA-Z0-9\-_]+$")
                    .WithMessage("El número de serie contiene caracteres no permitidos");
            });

            // Validación de FirmwareVersion (opcional)
            When(x => !string.IsNullOrEmpty(x.FirmwareVersion), () =>
            {
                RuleFor(x => x.FirmwareVersion)
                    .MaximumLength(50)
                    .WithMessage("La versión de firmware no puede exceder 50 caracteres")
                    .Matches(@"^[a-zA-Z0-9\-_\.]+$")
                    .WithMessage("La versión de firmware contiene caracteres no permitidos");
            });

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
        /// Valida que el tipo de dispositivo sea uno de los permitidos
        /// </summary>
        private static bool BeValidDeviceType(string deviceType)
        {
            var validTypes = new[]
            {
                "Fingerprint",
                "CardReader",
                "Biometric",
                "RFID",
                "QRCode",
                "Barcode",
                "Other"
            };

            return validTypes.Contains(deviceType, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida que la dirección IP tenga un formato válido
        /// </summary>
        private static bool BeValidIpAddress(string ipAddress)
        {
            // Validar formato IPv4 o IPv6 básico
            var ipv4Pattern = @"^(\d{1,3}\.){3}\d{1,3}$";
            var ipv6Pattern = @"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$";
            
            return System.Text.RegularExpressions.Regex.IsMatch(ipAddress, ipv4Pattern) ||
                   System.Text.RegularExpressions.Regex.IsMatch(ipAddress, ipv6Pattern);
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
