using WTB.API.Models.DTOs.Project;
using WTB.API.Models.Entities;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de proyectos que encapsula la lógica de negocio
    /// </summary>
    public interface IProjectService
    {
        #region Operaciones CRUD Básicas
        
        /// <summary>
        /// Obtiene un proyecto por ID
        /// </summary>
        Task<Projects?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene un proyecto por ID con includes
        /// </summary>
        Task<Projects?> GetByIdWithIncludesAsync(int id);
        
        /// <summary>
        /// Obtiene todos los proyectos
        /// </summary>
        Task<IEnumerable<Projects>> GetAllAsync();
        
        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        Task<Projects> CreateAsync(CreateProjectDto createDto);
        
        /// <summary>
        /// Actualiza un proyecto existente
        /// </summary>
        Task<Projects> UpdateAsync(int id, UpdateProjectDto updateDto);
        
        /// <summary>
        /// Elimina un proyecto
        /// </summary>
        Task DeleteAsync(int id);
        
        #endregion
        
        #region Operaciones de Búsqueda y Filtrado
        
        /// <summary>
        /// Obtiene proyectos con filtros avanzados y paginación
        /// </summary>
        Task<(IEnumerable<Projects> Items, int TotalCount)> GetFilteredAsync(ProjectFilterDto filter);
        
        /// <summary>
        /// Busca proyectos por término de búsqueda
        /// </summary>
        Task<IEnumerable<Projects>> SearchAsync(string searchTerm);
        
        /// <summary>
        /// Obtiene proyectos por estado
        /// </summary>
        Task<IEnumerable<Projects>> GetByStateAsync(int stateId);
        
        /// <summary>
        /// Obtiene proyectos activos
        /// </summary>
        Task<IEnumerable<Projects>> GetActiveAsync();
        
        /// <summary>
        /// Obtiene proyectos por rango de fechas
        /// </summary>
        Task<IEnumerable<Projects>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        
        #endregion
        
        #region Operaciones Específicas del Negocio
        
        /// <summary>
        /// Obtiene un proyecto por nombre
        /// </summary>
        Task<Projects?> GetByNameAsync(string projectName);
        
        /// <summary>
        /// Verifica si existe un proyecto con el nombre especificado
        /// </summary>
        Task<bool> ExistsByNameAsync(string projectName);
        
        /// <summary>
        /// Obtiene proyectos que requieren atención (fechas límite próximas)
        /// </summary>
        Task<IEnumerable<Projects>> GetNeedingAttentionAsync(int daysThreshold = 7);
        
        /// <summary>
        /// Obtiene proyectos por empleado asignado
        /// </summary>
        Task<IEnumerable<Projects>> GetByEmployeeAsync(int employeeId);
        
        /// <summary>
        /// Obtiene proyectos por rango de horas trabajadas
        /// </summary>
        Task<IEnumerable<Projects>> GetByHoursWorkedRangeAsync(decimal minHours, decimal maxHours);
        
        #endregion
        
        #region Operaciones de Asignación de Empleados
        
        /// <summary>
        /// Asigna un empleado a un proyecto
        /// </summary>
        Task AssignEmployeeAsync(int projectId, int employeeId, DateTime startDate, DateTime? endDate = null);
        
        /// <summary>
        /// Desasigna un empleado de un proyecto
        /// </summary>
        Task UnassignEmployeeAsync(int projectId, int employeeId);
        
        /// <summary>
        /// Obtiene empleados asignados a un proyecto
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetAssignedEmployeesAsync(int projectId);
        
        /// <summary>
        /// Obtiene proyectos asignados a un empleado
        /// </summary>
        Task<IEnumerable<Projects>> GetProjectsByEmployeeAsync(int employeeId);
        
        #endregion
        
        #region Operaciones de Reportes y Estadísticas
        
        /// <summary>
        /// Obtiene proyectos con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<ProjectWorkSummary>> GetTopWorkingAsync(DateTime fromDate, DateTime toDate, int top = 10);
        
        /// <summary>
        /// Obtiene estadísticas de proyectos
        /// </summary>
        Task<ProjectStatistics> GetStatisticsAsync();
        
        /// <summary>
        /// Obtiene el progreso de un proyecto
        /// </summary>
        Task<decimal> GetProgressAsync(int projectId);
        
        /// <summary>
        /// Obtiene el tiempo estimado de finalización de un proyecto
        /// </summary>
        Task<DateTime?> GetEstimatedEndDateAsync(int projectId);
        
        #endregion
        
        #region Operaciones de Validación
        
        /// <summary>
        /// Valida si un proyecto puede ser creado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CreateProjectDto createDto);
        
        /// <summary>
        /// Valida si un proyecto puede ser actualizado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateProjectDto updateDto);
        
        /// <summary>
        /// Valida si un proyecto puede ser eliminado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id);
        
        /// <summary>
        /// Valida si un empleado puede ser asignado a un proyecto
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateEmployeeAssignmentAsync(int projectId, int employeeId);
        
        #endregion
    }
}
