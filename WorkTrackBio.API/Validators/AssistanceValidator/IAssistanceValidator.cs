using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Assistance;

namespace WorkTrackBio.API.Validators.AssistanceValidator
{
    /// <summary>
    /// Interfaz para el validador de Assistance
    /// </summary>
    public interface IAssistanceValidator
    {
        /// <summary>
        /// Valida el DTO de creación
        /// </summary>
        Task<ValidationResult> ValidateCreateAsync(CreateAssistanceDataTransferObject createDto);

        /// <summary>
        /// Valida el DTO de actualización
        /// </summary>
        Task<ValidationResult> ValidateUpdateAsync(int id, UpdateAssistanceDataTransferObject updateDto);

        /// <summary>
        /// Valida que el empleado exista
        /// </summary>
        Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId);

        /// <summary>
        /// Valida que el proyecto exista
        /// </summary>
        Task<ValidationResult> ValidateProjectExistsAsync(int projectId);

        /// <summary>
        /// Valida la lógica de negocio para CheckIn/CheckOut
        /// </summary>
        Task<ValidationResult> ValidateCheckInOutLogicAsync(int employeeId, DateTime? checkOut);
    }
}
