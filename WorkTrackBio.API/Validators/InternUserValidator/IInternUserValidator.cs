using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.InternUser;

namespace WorkTrackBio.API.Validators.InternUserValidator
{
    /// <summary>
    /// Interfaz para el validator de usuarios internos
    /// </summary>
    public interface IInternUserValidator
    {
        /// <summary>
        /// Valida los datos para crear un usuario interno
        /// </summary>
        /// <param name="createDto">DTO con los datos para crear el usuario</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateCreateAsync(CreateInternUserDataTransferObject createDto);

        /// <summary>
        /// Valida los datos para actualizar un usuario interno
        /// </summary>
        /// <param name="updateDto">DTO con los datos para actualizar el usuario</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateUpdateAsync(UpdateInternUserDataTransferObject updateDto);

        /// <summary>
        /// Valida que el email sea único
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <param name="excludeUserId">ID del usuario a excluir de la validación (para updates)</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateEmailAsync(string email, int? excludeUserId = null);

        /// <summary>
        /// Valida que el rol exista
        /// </summary>
        /// <param name="roleId">ID del rol a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateRoleExistsAsync(int roleId);

        /// <summary>
        /// Valida que el estado exista
        /// </summary>
        /// <param name="stateId">ID del estado a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateStateExistsAsync(int stateId);

        /// <summary>
        /// Valida los campos de documento
        /// </summary>
        /// <param name="documentNumber">Número de documento a validar</param>
        /// <param name="documentTypeId">ID del tipo de documento a validar</param>
        /// <param name="documentExpire">Fecha de expiración del documento a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateDocumentAsync(string? documentNumber, int? documentTypeId, DateOnly? documentExpire = null);

        /// <summary>
        /// Valida la fecha de expiración del documento
        /// </summary>
        /// <param name="documentExpire">Fecha de expiración a validar</param>
        /// <returns>Resultado de la validación</returns>
        ValidationResult ValidateDocumentExpiration(DateOnly documentExpire);
    }
}
