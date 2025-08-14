using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Helpers;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de asignaciones de proyectos
    /// </summary>
    public interface IProjectAssignService
    {
        /// <summary>
        /// Obtiene asignaciones filtradas con paginación
        /// </summary>
        Task<PagedResponse<ProjectAssignListDto>> GetFilteredAsync(ProjectAssignFilterDto filter);

        /// <summary>
        /// Obtiene una asignación por ID
        /// </summary>
        Task<ProjectAssignDetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva asignación
        /// </summary>
        Task<ProjectAssignResponseDto> CreateAsync(CreateProjectAssignDto createDto);

        /// <summary>
        /// Actualiza una asignación existente
        /// </summary>
        Task<ProjectAssignResponseDto> UpdateAsync(UpdateProjectAssignDto updateDto);

        /// <summary>
        /// Elimina una asignación
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Obtiene asignaciones por empleado
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene asignaciones por proyecto
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene asignaciones activas
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetActiveAsync();

        /// <summary>
        /// Obtiene asignaciones por rango de fechas
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene asignaciones por estado del proyecto
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetByProjectStateAsync(string projectState);

        /// <summary>
        /// Obtiene asignaciones por estado del empleado
        /// </summary>
        Task<IEnumerable<ProjectAssignResponseDto>> GetByEmployeeStateAsync(string employeeState);

        /// <summary>
        /// Verifica si un empleado está asignado a un proyecto
        /// </summary>
        Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Obtiene estadísticas de asignaciones
        /// </summary>
        Task<ProjectAssignStatsDto> GetStatisticsAsync();

        /// <summary>
        /// Finaliza una asignación estableciendo fecha de fin
        /// </summary>
        Task<ProjectAssignResponseDto> EndAssignmentAsync(int id, DateTime endDate);

        /// <summary>
        /// Reactiva una asignación removiendo fecha de fin
        /// </summary>
        Task<ProjectAssignResponseDto> ReactivateAssignmentAsync(int id);
    }
}
