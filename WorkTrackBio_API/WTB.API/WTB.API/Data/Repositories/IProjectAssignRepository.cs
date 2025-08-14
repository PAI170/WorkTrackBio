using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de asignaciones de proyectos
    /// </summary>
    public interface IProjectAssignRepository : IRepository<ProjectsAssigns>
    {
        /// <summary>
        /// Obtiene asignaciones filtradas con paginación
        /// </summary>
        Task<PagedResponse<ProjectsAssigns>> GetFilteredAsync(ProjectAssignFilterDto filter);

        /// <summary>
        /// Obtiene una asignación por ID con includes
        /// </summary>
        Task<ProjectsAssigns?> GetByIdWithIncludesAsync(int id);

        /// <summary>
        /// Obtiene asignaciones por empleado
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene asignaciones por proyecto
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene asignaciones activas (sin fecha de fin)
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetActiveAsync();

        /// <summary>
        /// Obtiene asignaciones por rango de fechas
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene asignaciones por estado del proyecto
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetByProjectStateAsync(string projectState);

        /// <summary>
        /// Obtiene asignaciones por estado del empleado
        /// </summary>
        Task<IEnumerable<ProjectsAssigns>> GetByEmployeeStateAsync(string employeeState);

        /// <summary>
        /// Verifica si un empleado está asignado a un proyecto en un rango de fechas
        /// </summary>
        Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Obtiene estadísticas de asignaciones
        /// </summary>
        Task<ProjectAssignStatsDto> GetStatisticsAsync();
    }

}
