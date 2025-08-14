using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de garantías de proyecto
    /// </summary>
    public interface IProjectWarrantyRepository : IRepository<ProjectWarranty>
    {
        /// <summary>
        /// Obtiene garantías de proyecto con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de garantías</returns>
        Task<PagedResponse<ProjectWarranty>> GetFilteredAsync(ProjectWarrantyFilterDto filter);

        /// <summary>
        /// Obtiene una garantía por ID con todas sus relaciones
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>Garantía con relaciones cargadas</returns>
        Task<ProjectWarranty?> GetByIdWithIncludesAsync(int id);

        /// <summary>
        /// Obtiene garantías por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de garantías del proyecto</returns>
        Task<IEnumerable<ProjectWarranty>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene garantías por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de garantías registradas por el empleado</returns>
        Task<IEnumerable<ProjectWarranty>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene garantías por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de garantías con el estado especificado</returns>
        Task<IEnumerable<ProjectWarranty>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene garantías por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de garantías en el rango de fechas</returns>
        Task<IEnumerable<ProjectWarranty>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene garantías por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de garantías en el rango de costos</returns>
        Task<IEnumerable<ProjectWarranty>> GetByCostRangeAsync(decimal minCost, decimal maxCost);

        /// <summary>
        /// Busca garantías por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de garantías que coinciden</returns>
        Task<IEnumerable<ProjectWarranty>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene estadísticas de garantías
        /// </summary>
        /// <returns>Estadísticas de garantías</returns>
        Task<object> GetStatisticsAsync();
    }
}
