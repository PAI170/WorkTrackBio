using FluentValidation.Results;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;

namespace WorkTrackBio.API.Validators.DocumentValidator
{
    public class DocumentValidator : IDocumentValidator
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public DocumentValidator(IDocumentTypeRepository documentTypeRepository)
        {
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
        }

        public async Task<DocumentValidationResult> ValidateDocumentAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El número de documento no puede estar vacío" };

            // Verificar que el tipo de documento exista en la BD
            var documentType = await _documentTypeRepository.GetByIdAsync(documentTypeId);
            if (documentType == null)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "Tipo de documento no válido" };

            // Limpiar el documento de espacios y guiones
            var cleanDocument = new string(documentNumber.Where(c => char.IsLetterOrDigit(c)).ToArray());

            // Validar según el nombre del tipo de documento
            return ValidateDocumentByTypeName(cleanDocument, documentType.DocumentName);
        }

        public DocumentValidationResult ValidateDocumentByTypeName(string documentNumber, string documentTypeName)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El número de documento no puede estar vacío" };

            // Normalizar el nombre del tipo para comparación
            var normalizedTypeName = documentTypeName.ToLower().Trim();

            return normalizedTypeName switch
            {
                "cédula de identidad" or "cedula de identidad" => ValidateCedula(documentNumber),
                "pasaporte" => ValidatePasaporte(documentNumber),
                "dimex" => ValidateDimex(documentNumber),
                "permiso de trabajo" => ValidatePermisoTrabajo(documentNumber),
                _ => new DocumentValidationResult { IsValid = false, ErrorMessage = $"Tipo de documento '{documentTypeName}' no soportado para validación" }
            };
        }

        public async Task<ValidationResult> ValidateDocumentTypeExistsAsync(int documentTypeId)
        {
            var result = new ValidationResult();
            
            if (documentTypeId <= 0)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", "El ID del tipo de documento debe ser mayor que 0"));
                return result;
            }

            var documentType = await _documentTypeRepository.GetByIdAsync(documentTypeId);
            if (documentType == null)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", $"No existe un tipo de documento con ID {documentTypeId}"));
            }

            return result;
        }

        private static DocumentValidationResult ValidateCedula(string documentNumber)
        {
            if (documentNumber.Length != 9)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "La cédula debe tener exactamente 9 dígitos" };

            if (!documentNumber.All(char.IsDigit))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "La cédula solo puede contener números" };

            // Validar que no sea todo ceros
            if (documentNumber.All(c => c == '0'))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "La cédula no puede ser todo ceros" };

            return new DocumentValidationResult { IsValid = true };
        }

        private static DocumentValidationResult ValidatePasaporte(string documentNumber)
        {
            if (documentNumber.Length < 6 || documentNumber.Length > 9)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El pasaporte debe tener entre 6 y 9 caracteres" };

            // Verificar que tenga 1 o 2 letras al inicio
            var letterCount = documentNumber.TakeWhile(char.IsLetter).Count();
            if (letterCount < 1 || letterCount > 2)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El pasaporte debe comenzar con 1 o 2 letras" };

            // Verificar que el resto sean números
            var remainingChars = documentNumber.Substring(letterCount);
            if (!remainingChars.All(char.IsDigit))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El pasaporte debe contener solo números después de las letras" };

            return new DocumentValidationResult { IsValid = true };
        }

        // Valida formato de DIMEX (12 dígitos)
        private static DocumentValidationResult ValidateDimex(string documentNumber)
        {
            if (documentNumber.Length != 12)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El DIMEX debe tener exactamente 12 dígitos" };

            if (!documentNumber.All(char.IsDigit))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El DIMEX solo puede contener números" };

            // Validar que no sea todo ceros
            if (documentNumber.All(c => c == '0'))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El DIMEX no puede ser todo ceros" };

            return new DocumentValidationResult { IsValid = true };
        }

        // Valida formato de Permiso de Trabajo (12 números)
        private static DocumentValidationResult ValidatePermisoTrabajo(string documentNumber)
        {
            if (documentNumber.Length != 12)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El permiso de trabajo debe tener exactamente 12 números" };

            if (!documentNumber.All(char.IsDigit))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El permiso de trabajo solo puede contener números" };

            // Validar que no sea todo ceros
            if (documentNumber.All(c => c == '0'))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El permiso de trabajo no puede ser todo ceros" };

            return new DocumentValidationResult { IsValid = true };
        }
    }
}
