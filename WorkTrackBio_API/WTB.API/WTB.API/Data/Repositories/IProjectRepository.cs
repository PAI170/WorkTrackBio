using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Project;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de proyectos con métodos específicos del negocio
    /// </summary>
    public interface IProjectRepository : IRepository<Projects>
    {
        /// <summary>
        /// Obtiene un proyecto por nombre
        /// </summary>
        Task<Projects?> GetByNameAsync(string projectName);

        /// <summary>
        /// Verifica si existe un proyecto con el nombre especificado
        /// </summary>
        Task<bool> ExistsByNameAsync(string projectName);

        /// <summary>
        /// Obtiene proyectos por estado
        /// </summary>
        Task<IEnumerable<Projects>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene proyectos activos
        /// </summary>
        Task<IEnumerable<Projects>> GetActiveProjectsAsync();

        /// <summary>
        /// Obtiene proyectos por cliente
        /// </summary>
        Task<IEnumerable<Projects>> GetByClientAsync(string clientName);

        /// <summary>
        /// Obtiene proyectos por rango de fechas
        /// </summary>
        Task<IEnumerable<Projects>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene proyectos con filtros avanzados y paginación
        /// </summary>
        Task<(IEnumerable<Projects> Items, int TotalCount)> GetFilteredAsync(
            ProjectFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Obtiene proyectos con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<ProjectWorkSummary>> GetTopWorkingProjectsAsync(DateTime fromDate, DateTime toDate, int top = 10);

        /// <summary>
        /// Obtiene estadísticas de proyectos
        /// </summary>
        Task<ProjectStatistics> GetProjectStatisticsAsync();

        /// <summary>
        /// Obtiene proyectos que requieren atención (fechas límite próximas)
        /// </summary>
        Task<IEnumerable<Projects>> GetProjectsNeedingAttentionAsync(int daysThreshold = 7);

        /// <summary>
        /// Obtiene proyectos por rango de presupuesto
        /// </summary>
        Task<IEnumerable<Projects>> GetByBudgetRangeAsync(decimal minBudget, decimal maxBudget);

        /// <summary>
        /// Busca proyectos por término de búsqueda
        /// </summary>
        Task<IEnumerable<Projects>> SearchProjectsAsync(string searchTerm);

        /// <summary>
        /// Obtiene proyectos asignados a un empleado
        /// </summary>
        Task<IEnumerable<Projects>> GetProjectsByEmployeeAsync(int employeeId);
    }
}
