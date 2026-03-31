using FluentValidation.Results;
using WorkTrackBio.API.Common;

namespace WorkTrackBio.API.Validators.DocumentValidator
{
    /// <summary>
    /// Interfaz para el validator de documentos siguiendo el patrón establecido
    /// </summary>
    public interface IDocumentValidator
    {
        /// <summary>
        /// Valida el formato de un documento según su tipo usando la BD
        /// </summary>
        /// <param name="documentNumber">Número de documento a validar</param>
        /// <param name="documentTypeId">ID del tipo de documento</param>
        /// <returns>Resultado de la validación con mensaje de error si aplica</returns>
        Task<DocumentValidationResult> ValidateDocumentAsync(string documentNumber, int documentTypeId);

        /// <summary>
        /// Valida el formato de un documento según su nombre de tipo
        /// </summary>
        /// <param name="documentNumber">Número de documento a validar</param>
        /// <param name="documentTypeName">Nombre del tipo de documento</param>
        /// <returns>Resultado de la validación con mensaje de error si aplica</returns>
        DocumentValidationResult ValidateDocumentByTypeName(string documentNumber, string documentTypeName);

        /// <summary>
        /// Valida que el tipo de documento exista en la BD
        /// </summary>
        /// <param name="documentTypeId">ID del tipo de documento</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateDocumentTypeExistsAsync(int documentTypeId);
    }
}
