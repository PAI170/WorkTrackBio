using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO para filtrar y buscar proyectos
    /// </summary>
    public record ProjectFilterDto(
        string? SearchTerm,
        
        int? StateId,
        
        DateTime? StartDateFrom,
        
        DateTime? StartDateTo,
        
        DateTime? EndDateFrom,
        
        DateTime? EndDateTo,
        
        bool? IsActive,
        
        bool? IsCompleted,
        
        bool? IsOverdue,
        
        int? EmployeeId,
        
        decimal? MinHoursWorked,
        
        decimal? MaxHoursWorked,
        
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero")]
        int Page = 1,
        
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        int PageSize = 10,
        
        string? SortBy = "StartDate",
        
        bool SortDescending = true
    );
}
