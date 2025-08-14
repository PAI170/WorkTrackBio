using WTB.API.Models.DTOs.InternUser;
using WTB.API.Models.Entities;
using WTB.API.Helpers;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de usuarios internos
    /// </summary>
    public interface IInternUserService
    {
        /// <summary>
        /// Obtiene usuarios internos con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de usuarios internos</returns>
        Task<PagedResponse<InternUserResponseDto>> GetFilteredAsync(InternUserFilterDto filter);

        /// <summary>
        /// Obtiene un usuario interno por ID
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>Usuario interno si existe</returns>
        Task<InternUserDetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo usuario interno
        /// </summary>
        /// <param name="createDto">Datos para crear el usuario interno</param>
        /// <returns>Usuario interno creado</returns>
        Task<InternUserResponseDto> CreateAsync(CreateInternUserDto createDto);

        /// <summary>
        /// Actualiza un usuario interno existente
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Usuario interno actualizado</returns>
        Task<InternUserResponseDto> UpdateAsync(int id, UpdateInternUserDto updateDto);

        /// <summary>
        /// Elimina un usuario interno (soft delete)
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Busca usuarios internos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de usuarios internos que coinciden</returns>
        Task<IEnumerable<InternUserListDto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene usuarios internos activos
        /// </summary>
        /// <returns>Lista de usuarios internos activos</returns>
        Task<IEnumerable<InternUserListDto>> GetActiveAsync();

        /// <summary>
        /// Obtiene usuarios internos por rol
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de usuarios internos con el rol especificado</returns>
        Task<IEnumerable<InternUserListDto>> GetByRoleAsync(int rolId);

        /// <summary>
        /// Cambia la contraseña de un usuario interno
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <param name="changePasswordDto">Datos para cambiar la contraseña</param>
        /// <returns>True si se cambió correctamente</returns>
        Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Valida los datos para crear un usuario interno
        /// </summary>
        /// <param name="createDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateInternUserDto createDto);

        /// <summary>
        /// Valida los datos para actualizar un usuario interno
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <param name="updateDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, UpdateInternUserDto updateDto);

        /// <summary>
        /// Valida si se puede eliminar un usuario interno
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id);
    }
}
