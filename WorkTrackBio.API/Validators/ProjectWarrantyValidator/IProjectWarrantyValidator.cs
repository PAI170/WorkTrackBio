using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Validators.ProjectWarrantyValidator
{
    /// <summary>
    /// Interfaz para el validador de ProjectWarranty
    /// </summary>
    public interface IProjectWarrantyValidator
    {
        /// <summary>
        /// Valida la creación de una garantía de proyecto
        /// </summary>
        Task<ValidationResult> ValidateCreateAsync(CreateProjectWarrantyDataTransferObject createDto);

        /// <summary>
        /// Valida la actualización de una garantía de proyecto
        /// </summary>
        Task<ValidationResult> ValidateUpdateAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto);

        /// <summary>
        /// Valida que el proyecto existe
        /// </summary>
        Task<ValidationResult> ValidateProjectExistsAsync(int projectId);

        /// <summary>
        /// Valida que el empleado existe
        /// </summary>
        Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId);

        /// <summary>
        /// Valida que el estado existe
        /// </summary>
        Task<ValidationResult> ValidateStateExistsAsync(int stateId);
    }
}
