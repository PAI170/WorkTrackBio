using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Validators.StateValidator
{
    /// <summary>
    /// Interfaz para el validator de State
    /// </summary>
    public interface IStateValidator
    {
        /// <summary>
        /// Valida los datos para crear un estado
        /// Incluye validación de duplicados de nombre
        /// </summary>
        /// <param name="createDto">DTO con los datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateCreateAsync(CreateStateDataTransferObject createDto);

        /// <summary>
        /// Valida los datos para actualizar un estado
        /// Incluye validación de existencia del ID y duplicados de nombre
        /// </summary>
        /// <param name="updateDto">DTO con los datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateUpdateAsync(UpdateStateDataTransferObject updateDto);

        /// <summary>
        /// Valida el ID de un estado (formato y rango)
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <returns>Resultado de la validación</returns>
        ValidationResult ValidateId(int id);

        /// <summary>
        /// Valida el nombre de un estado (formato y longitud)
        /// </summary>
        /// <param name="stateName">Nombre a validar</param>
        /// <returns>Resultado de la validación</returns>
        ValidationResult ValidateStateName(string stateName);

        /// <summary>
        /// Valida el tipo de estado (formato, longitud y valores permitidos)
        /// </summary>
        /// <param name="stateType">Tipo a validar</param>
        /// <returns>Resultado de la validación</returns>
        ValidationResult ValidateStateType(string stateType);

        /// <summary>
        /// Valida si un ID de estado existe en la base de datos
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateStateExistsAsync(int id);

        /// <summary>
        /// Valida si un nombre de estado ya existe (para evitar duplicados)
        /// </summary>
        /// <param name="stateName">Nombre a validar</param>
        /// <param name="excludeId">ID a excluir de la validación (para updates)</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateStateNameUniqueAsync(string stateName, int? excludeId = null);
    }
}
