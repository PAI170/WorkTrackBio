using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Employee
{
    /// <summary>
    /// DTO para filtrar y buscar empleados
    /// </summary>
    public record EmployeeFilterDto(
        string? SearchTerm,
        
        int? DocumentTypeId,
        
        int? StateId,
        
        DateTime? RegisterDateFrom,
        
        DateTime? RegisterDateTo,
        
        DateTime? BirthdayFrom,
        
        DateTime? BirthdayTo,
        
        decimal? CostPerHourMin,
        
        decimal? CostPerHourMax,
        
        bool? HasFingerPrint,
        
        bool? IsActive,
        
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero")]
        int Page = 1,
        
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        int PageSize = 10,
        
        string? SortBy = "RegisterDate",
        
        bool SortDescending = true
    );
}
