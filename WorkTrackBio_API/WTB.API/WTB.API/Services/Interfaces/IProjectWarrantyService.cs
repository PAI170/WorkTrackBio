using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Models.Entities;
using WTB.API.Helpers;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de garantías de proyecto
    /// </summary>
    public interface IProjectWarrantyService
    {
        /// <summary>
        /// Obtiene garantías de proyecto con filtros aplicados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de garantías</returns>
        Task<PagedResponse<ProjectWarrantyResponseDto>> GetFilteredAsync(ProjectWarrantyFilterDto filter);

        /// <summary>
        /// Obtiene una garantía por ID
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>Garantía si existe</returns>
        Task<ProjectWarrantyDetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        /// <param name="createDto">Datos para crear la garantía</param>
        /// <returns>Garantía creada</returns>
        Task<ProjectWarrantyResponseDto> CreateAsync(CreateProjectWarrantyDto createDto);

        /// <summary>
        /// Actualiza una garantía existente
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Garantía actualizada</returns>
        Task<ProjectWarrantyResponseDto> UpdateAsync(int id, CreateProjectWarrantyDto updateDto);

        /// <summary>
        /// Elimina una garantía (soft delete)
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Obtiene garantías por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de garantías del proyecto</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene garantías por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de garantías registradas por el empleado</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene garantías por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de garantías con el estado especificado</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene garantías por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de garantías en el rango de fechas</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene garantías por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de garantías en el rango de costos</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> GetByCostRangeAsync(decimal minCost, decimal maxCost);

        /// <summary>
        /// Busca garantías por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de garantías que coinciden</returns>
        Task<IEnumerable<ProjectWarrantyListDto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Obtiene estadísticas de garantías
        /// </summary>
        /// <returns>Estadísticas de garantías</returns>
        Task<object> GetStatisticsAsync();

        /// <summary>
        /// Valida los datos para crear una garantía
        /// </summary>
        /// <param name="createDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateProjectWarrantyDto createDto);

        /// <summary>
        /// Valida los datos para actualizar una garantía
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <param name="updateDto">Datos a validar</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, CreateProjectWarrantyDto updateDto);

        /// <summary>
        /// Valida si se puede eliminar una garantía
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>Resultado de la validación</returns>
        Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id);
    }
}
