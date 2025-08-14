using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Models.Entities;
using WTB.API.Helpers;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de mantenimientos de proyecto
    /// </summary>
    public interface IProjectMaintenanceService
    {
        /// <summary>
        /// Obtiene mantenimientos de proyecto con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de mantenimientos</returns>
        Task<PagedResponse<ProjectMaintenanceResponseDto>> GetFilteredAsync(ProjectMaintenanceFilterDto filter);

        /// <summary>
        /// Obtiene un mantenimiento por ID
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>Mantenimiento si existe</returns>
        Task<ProjectMaintenanceDetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo mantenimiento de proyecto
        /// </summary>
        /// <param name="createDto">Datos para crear el mantenimiento</param>
        /// <returns>Mantenimiento creado</returns>
        Task<ProjectMaintenanceResponseDto> CreateAsync(CreateProjectMaintenanceDto createDto);

        /// <summary>
        /// Actualiza un mantenimiento existente
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Mantenimiento actualizado</returns>
        Task<ProjectMaintenanceResponseDto> UpdateAsync(int id, CreateProjectMaintenanceDto updateDto);

        /// <summary>
        /// Elimina un mantenimiento (soft delete)
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Obtiene mantenimientos por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de mantenimientos del proyecto</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene mantenimientos por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de mantenimientos realizados por el empleado</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene mantenimientos por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de mantenimientos con el estado especificado</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene mantenimientos por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de mantenimientos en el rango de fechas</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene mantenimientos por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de mantenimientos en el rango de costos</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> GetByCostRangeAsync(decimal minCost, decimal maxCost);

        /// <summary>
        /// Busca mantenimientos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de mantenimientos que coinciden</returns>
        Task<IEnumerable<ProjectMaintenanceListDto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene estadísticas de mantenimientos
        /// </summary>
        /// <returns>Estadísticas de mantenimientos</returns>
        Task<object> GetStatisticsAsync();

        /// <summary>
        /// Valida los datos para crear un mantenimiento
        /// </summary>
        /// <param name="createDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateProjectMaintenanceDto createDto);

        /// <summary>
        /// Valida los datos para actualizar un mantenimiento
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <param name="updateDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, CreateProjectMaintenanceDto updateDto);

        /// <summary>
        /// Valida si se puede eliminar un mantenimiento
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id);
    }
}
