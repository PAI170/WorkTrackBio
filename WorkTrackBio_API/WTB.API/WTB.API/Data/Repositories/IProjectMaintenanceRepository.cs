using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de mantenimientos de proyecto
    /// </summary>
    public interface IProjectMaintenanceRepository : IRepository<ProjectMaintenance>
    {
        /// <summary>
        /// Obtiene mantenimientos de proyecto con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de mantenimientos</returns>
        Task<PagedResponse<ProjectMaintenance>> GetFilteredAsync(ProjectMaintenanceFilterDto filter);

        /// <summary>
        /// Obtiene un mantenimiento por ID con todas sus relaciones
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>Mantenimiento con relaciones cargadas</returns>
        Task<ProjectMaintenance?> GetByIdWithIncludesAsync(int id);

        /// <summary>
        /// Obtiene mantenimientos por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de mantenimientos del proyecto</returns>
        Task<IEnumerable<ProjectMaintenance>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene mantenimientos por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de mantenimientos realizados por el empleado</returns>
        Task<IEnumerable<ProjectMaintenance>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene mantenimientos por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de mantenimientos con el estado especificado</returns>
        Task<IEnumerable<ProjectMaintenance>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene mantenimientos por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de mantenimientos en el rango de fechas</returns>
        Task<IEnumerable<ProjectMaintenance>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene mantenimientos por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de mantenimientos en el rango de costos</returns>
        Task<IEnumerable<ProjectMaintenance>> GetByCostRangeAsync(decimal minCost, decimal maxCost);

        /// <summary>
        /// Busca mantenimientos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de mantenimientos que coinciden</returns>
        Task<IEnumerable<ProjectMaintenance>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene estadísticas de mantenimientos
        /// </summary>
        /// <returns>Estadísticas de mantenimientos</returns>
        Task<object> GetStatisticsAsync();
    }
}
