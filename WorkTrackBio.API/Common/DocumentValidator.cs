namespace WorkTrackBio.API.Common
{
    /// <summary>
    /// Utilidades para validar diferentes tipos de documentos según las reglas de Costa Rica
    /// </summary>
    public static class DocumentValidator
    {
        /// <summary>
        /// Valida el formato de un documento según su tipo
        /// </summary>
        /// <param name="documentNumber">Número de documento a validar</param>
        /// <param name="documentTypeId">ID del tipo de documento</param>
        /// <returns>Resultado de la validación con mensaje de error si aplica</returns>
        public static DocumentValidationResult ValidateDocument(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El número de documento no puede estar vacío" };

            // Limpiar el documento de espacios y guiones
            var cleanDocument = new string(documentNumber.Where(c => char.IsLetterOrDigit(c)).ToArray());

            return documentTypeId switch
            {
                1 => ValidateCedula(cleanDocument),        // Cédula de Identidad
                2 => ValidatePasaporte(cleanDocument),     // Pasaporte
                3 => ValidateDimex(cleanDocument),         // DIMEX
                4 => ValidatePermisoTrabajo(cleanDocument), // Permiso de Trabajo
                _ => new DocumentValidationResult { IsValid = false, ErrorMessage = "Tipo de documento no válido" }
            };
        }

        /// <summary>
        /// Valida formato de Cédula de Identidad costarricense (9 dígitos)
        /// </summary>
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

        /// <summary>
        /// Valida formato de Pasaporte (formato estándar internacional)
        /// </summary>
        private static DocumentValidationResult ValidatePasaporte(string documentNumber)
        {
            if (documentNumber.Length < 6 || documentNumber.Length > 9)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El pasaporte debe tener entre 6 y 9 caracteres" };

            // El pasaporte puede contener letras y números
            if (!documentNumber.All(c => char.IsLetterOrDigit(c)))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El pasaporte solo puede contener letras y números" };

            return new DocumentValidationResult { IsValid = true };
        }

        /// <summary>
        /// Valida formato de DIMEX (12 dígitos)
        /// </summary>
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

        /// <summary>
        /// Valida formato de Permiso de Trabajo (formato específico)
        /// </summary>
        private static DocumentValidationResult ValidatePermisoTrabajo(string documentNumber)
        {
            if (documentNumber.Length < 6 || documentNumber.Length > 10)
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El permiso de trabajo debe tener entre 6 y 10 caracteres" };

            // El permiso de trabajo puede contener letras y números
            if (!documentNumber.All(c => char.IsLetterOrDigit(c)))
                return new DocumentValidationResult { IsValid = false, ErrorMessage = "El permiso de trabajo solo puede contener letras y números" };

            return new DocumentValidationResult { IsValid = true };
        }

        /// <summary>
        /// Obtiene el nombre del tipo de documento por ID
        /// </summary>
        public static string GetDocumentTypeName(int documentTypeId)
        {
            return documentTypeId switch
            {
                1 => "Cédula de Identidad",
                2 => "Pasaporte",
                3 => "DIMEX",
                4 => "Permiso de Trabajo",
                _ => "Tipo de documento desconocido"
            };
        }
    }

    /// <summary>
    /// Resultado de la validación de un documento
    /// </summary>
    public class DocumentValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
