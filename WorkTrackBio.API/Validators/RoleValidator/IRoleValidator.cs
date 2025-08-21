using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Role;

namespace WorkTrackBio.API.Validators.RoleValidator
{
    /// <summary>
    /// Interfaz para el validator de roles
    /// </summary>
    public interface IRoleValidator
    {
        /// <summary>
        /// Valida los datos para crear un rol
        /// </summary>
        Task<ValidationResult> ValidateCreateAsync(CreateRoleDataTransferObject createDto);

        /// <summary>
        /// Valida los datos para actualizar un rol
        /// </summary>
        Task<ValidationResult> ValidateUpdateAsync(UpdateRoleDataTransferObject updateDto);

        /// <summary>
        /// Valida el ID de un rol
        /// </summary>
        ValidationResult ValidateId(int id);

        /// <summary>
        /// Valida el nombre de un rol
        /// </summary>
        ValidationResult ValidateRoleName(string roleName);

        /// <summary>
        /// Valida si un ID de rol existe en la base de datos
        /// </summary>
        Task<ValidationResult> ValidateRoleExistsAsync(int id);

        /// <summary>
        /// Valida si un nombre de rol ya existe (para evitar duplicados)
        /// </summary>
        Task<ValidationResult> ValidateRoleNameUniqueAsync(string roleName, int? excludeId = null);
    }
}
