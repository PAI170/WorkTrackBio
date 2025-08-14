using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de usuarios internos
    /// </summary>
    public interface IInternUserRepository : IRepository<InternUsers>
    {
        /// <summary>
        /// Obtiene usuarios internos con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de usuarios internos</returns>
        Task<PagedResponse<InternUsers>> GetFilteredAsync(InternUserFilterDto filter);

        /// <summary>
        /// Obtiene un usuario interno por ID con todas sus relaciones
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>Usuario interno con relaciones cargadas</returns>
        Task<InternUsers?> GetByIdWithIncludesAsync(int id);

        /// <summary>
        /// Obtiene un usuario interno por email
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>Usuario interno si existe</returns>
        Task<InternUsers?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica si existe un email
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="excludeId">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si existe, false en caso contrario</returns>
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);

        /// <summary>
        /// Busca usuarios internos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de usuarios internos que coinciden</returns>
        Task<IEnumerable<InternUsers>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene usuarios internos activos
        /// </summary>
        /// <returns>Lista de usuarios internos activos</returns>
        Task<IEnumerable<InternUsers>> GetActiveAsync();

        /// <summary>
        /// Obtiene usuarios internos por rol
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de usuarios internos con el rol especificado</returns>
        Task<IEnumerable<InternUsers>> GetByRoleAsync(int rolId);
    }
}
