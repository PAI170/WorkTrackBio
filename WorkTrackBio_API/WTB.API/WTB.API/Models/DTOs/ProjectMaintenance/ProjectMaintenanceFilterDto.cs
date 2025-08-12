using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectMaintenance
{
    /// <summary>
    /// DTO para filtrar y buscar mantenimientos de proyecto
    /// </summary>
    public record ProjectMaintenanceFilterDto(
        // Filtros por proyecto
        int? ProjectId = null,
        string? ProjectName = null,
        
        // Filtros por empleado
        int? EmployeeId = null,
        string? EmployeeName = null,
        
        // Filtros por estado
        int? StateId = null,
        string? StateName = null,
        
        // Filtros por costo
        decimal? MinCost = null,
        decimal? MaxCost = null,
        
        // Filtros por fecha
        DateTime? MaintenanceDateFrom = null,
        DateTime? MaintenanceDateTo = null,
        DateTime? CreatedFrom = null,
        DateTime? CreatedTo = null,
        
        // Filtros de texto
        string? SearchText = null,
        
        // Paginación
        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        int PageSize = 20,
        
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        int PageNumber = 1
    );
}
