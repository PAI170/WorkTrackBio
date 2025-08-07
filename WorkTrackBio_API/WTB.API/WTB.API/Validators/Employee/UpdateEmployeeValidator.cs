using FluentValidation;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Models.Enums;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.Employee
{
    /// <summary>
    /// Validador para la actualización de empleados
    /// </summary>
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeValidator()
        {
            // Validación del ID
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser mayor que cero");

            // Validación del número de documento basada en el tipo
            RuleFor(x => x)
                .Must(x => BeValidDocumentNumberForType(x.DocumentNumber, x.DocumentTypeId))
                .WithMessage(x => GetDocumentValidationMessage(x.DocumentTypeId))
                .WithName("DocumentNumber");

            // Validación de nombres
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio")
                .Must(BeValidName)
                .WithMessage("El nombre solo puede contener letras, espacios y acentos");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido es obligatorio")
                .Must(BeValidName)
                .WithMessage("El apellido solo puede contener letras, espacios y acentos");

            // Validación de fecha de nacimiento
            RuleFor(x => x.Birthday)
                .NotEmpty()
                .WithMessage("La fecha de nacimiento es obligatoria")
                .Must(BeValidBirthday)
                .WithMessage("La fecha de nacimiento debe ser válida (mayor de 18 años y menor de 75 años)")
                .LessThan(DateTime.Today.AddYears(-18))
                .WithMessage("El empleado debe ser mayor de 18 años")
                .GreaterThan(DateTime.Today.AddYears(-75))
                .WithMessage("La fecha de nacimiento no puede ser mayor a 75 años");

            // Validación de teléfono (formato Costa Rica: nnnn-nnnn o nnnnnnnn)
            RuleFor(x => x.PhoneNumber)
                .Must(BeValidCostaRicanPhone)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("El formato del teléfono debe ser nnnn-nnnn o nnnnnnnn (8 dígitos, ej: 2456-7890 o 24567890)");

            // Validación de teléfono de emergencia
            RuleFor(x => x.EmergencyContactPhoneNumber)
                .Must(BeValidCostaRicanPhone)
                .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhoneNumber))
                .WithMessage("El formato del teléfono de emergencia debe ser nnnn-nnnn o nnnnnnnn (8 dígitos)");

            // Validación de IBAN (formato Costa Rica: CRnnnnnnnnnnnnnnnnnnnn)
            RuleFor(x => x.IBAN)
                .Must(BeValidCostaRicanIBAN)
                .When(x => !string.IsNullOrEmpty(x.IBAN))
                .WithMessage("El formato del IBAN debe ser válido para Costa Rica (CR + 20 dígitos)");

            // Validación de costo por hora
            RuleFor(x => x.CostPerHour)
                .GreaterThan(0)
                .When(x => x.CostPerHour.HasValue)
                .WithMessage("El costo por hora debe ser mayor que cero")
                .LessThanOrEqualTo(20000)
                .When(x => x.CostPerHour.HasValue)
                .WithMessage("El costo por hora no puede exceder ₡20.000");

            // Validación de fecha de expiración del documento
            RuleFor(x => x.DocumentExpire)
                .GreaterThan(DateTime.Today)
                .When(x => x.DocumentExpire.HasValue)
                .WithMessage("La fecha de expiración del documento debe ser futura");

            // Validación de contacto de emergencia
            RuleFor(x => x.EmergencyContact)
                .Must(BeValidName)
                .When(x => !string.IsNullOrEmpty(x.EmergencyContact))
                .WithMessage("El contacto de emergencia solo puede contener letras, espacios y acentos");
        }

        /// <summary>
        /// Valida que el número de documento coincida con el formato del tipo seleccionado
        /// </summary>
        private bool BeValidDocumentNumberForType(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            return (DocumentTypeEnum)documentTypeId switch
            {
                DocumentTypeEnum.CedulaIdentidad => IsValidCedulaIdentidad(documentNumber),
                DocumentTypeEnum.Pasaporte => IsValidPasaporte(documentNumber),
                DocumentTypeEnum.DIMEX => IsValidDIMEX(documentNumber),
                DocumentTypeEnum.PermisoTrabajo => IsValidPermisoTrabajo(documentNumber),
                _ => false // Tipo de documento no válido
            };
        }

        /// <summary>
        /// Obtiene el mensaje de error específico según el tipo de documento
        /// </summary>
        private string GetDocumentValidationMessage(int documentTypeId)
        {
            return (DocumentTypeEnum)documentTypeId switch
            {
                DocumentTypeEnum.CedulaIdentidad => "El formato de la cédula debe ser: n-nnnn-nnnn o nnnnnnnnn (ej: 1-1234-5678 o 112345678)",
                DocumentTypeEnum.Pasaporte => "El formato del pasaporte debe ser: 1-2 letras seguidas de 6-8 números (ej: A1234567, CR12345678)",
                DocumentTypeEnum.DIMEX => "El formato del DIMEX debe ser: DIM-nnnnnnnn (ej: DIM-12345678)",
                DocumentTypeEnum.PermisoTrabajo => "El formato del permiso de trabajo debe ser: PT-nnnnnn (ej: PT-123456)",
                _ => "Tipo de documento no válido"
            };
        }

        /// <summary>
        /// Valida formato de Cédula de Identidad CR: n-nnnn-nnnn o nnnnnnnnn (9 dígitos)
        /// </summary>
        private bool IsValidCedulaIdentidad(string documentNumber)
        {
            // Formato con guiones: n-nnnn-nnnn
            var patternWithDashes = @"^[1-9]-\d{4}-\d{4}$";
            
            // Formato sin guiones: nnnnnnnnn (9 dígitos, no puede empezar con 0)
            var patternWithoutDashes = @"^[1-9]\d{8}$";
            
            return Regex.IsMatch(documentNumber, patternWithDashes) || 
                   Regex.IsMatch(documentNumber, patternWithoutDashes);
        }

        /// <summary>
        /// Valida formato de Pasaporte: 1-2 letras + 6-8 números
        /// </summary>
        private bool IsValidPasaporte(string documentNumber)
        {
            var pattern = @"^[A-Z]{1,2}\d{6,8}$";
            return Regex.IsMatch(documentNumber.ToUpper(), pattern);
        }

        /// <summary>
        /// Valida formato de DIMEX: DIM-nnnnnnnn
        /// </summary>
        private bool IsValidDIMEX(string documentNumber)
        {
            var pattern = @"^DIM-\d{8}$";
            return Regex.IsMatch(documentNumber.ToUpper(), pattern);
        }

        /// <summary>
        /// Valida formato de Permiso de Trabajo: PT-nnnnnn
        /// </summary>
        private bool IsValidPermisoTrabajo(string documentNumber)
        {
            var pattern = @"^PT-\d{6}$";
            return Regex.IsMatch(documentNumber.ToUpper(), pattern);
        }

        /// <summary>
        /// Valida que el nombre solo contenga caracteres válidos
        /// </summary>
        private bool BeValidName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Solo letras, espacios, acentos y algunos caracteres especiales comunes en nombres
            var namePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-\.\']+$";
            return Regex.IsMatch(name, namePattern);
        }

        /// <summary>
        /// Valida que la fecha de nacimiento sea lógica
        /// </summary>
        private bool BeValidBirthday(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            
            if (birthday.Date > today.AddYears(-age))
                age--;

            return age >= 18 && age <= 75;
        }

        /// <summary>
        /// Valida el formato de teléfono costarricense (nnnn-nnnn o nnnnnnnn - 8 dígitos)
        /// </summary>
        private bool BeValidCostaRicanPhone(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return true; // Es opcional

            // Formato con guión: nnnn-nnnn
            var phonePatternWithDash = @"^\d{4}-\d{4}$";
            
            // Formato sin guión: nnnnnnnn (8 dígitos)
            var phonePatternWithoutDash = @"^\d{8}$";
            
            return Regex.IsMatch(phoneNumber, phonePatternWithDash) || 
                   Regex.IsMatch(phoneNumber, phonePatternWithoutDash);
        }

        /// <summary>
        /// Valida el formato de IBAN costarricense (CR + 20 dígitos)
        /// </summary>
        private bool BeValidCostaRicanIBAN(string? iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return true; // Es opcional

            var ibanPattern = @"^CR\d{20}$";
            return Regex.IsMatch(iban, ibanPattern);
        }
    }
}
