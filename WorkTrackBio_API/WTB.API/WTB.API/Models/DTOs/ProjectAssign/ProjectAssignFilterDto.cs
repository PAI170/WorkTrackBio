using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para filtrar y buscar asignaciones empleado-proyecto
    /// </summary>
    public record ProjectAssignFilterDto(
        string? SearchTerm,
        
        int? EmployeeId,
        
        string? EmployeeName,
        
        int? ProjectId,
        
        string? ProjectName,
        
        bool? IsActive,
        
        DateTime? AssignedFrom,
        
        DateTime? AssignedTo,
        
        DateTime? EndDateFrom,
        
        DateTime? EndDateTo,
        
        bool? IsOverdue,
        
        decimal? MinHoursWorked,
        
        decimal? MaxHoursWorked,
        
        string? ProjectState,
        
        string? EmployeeState,
        
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero")]
        int Page = 1,
        
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        int PageSize = 10,
        
        string? SortBy = "AssignDate",
        
        bool SortDescending = true
    );
}
