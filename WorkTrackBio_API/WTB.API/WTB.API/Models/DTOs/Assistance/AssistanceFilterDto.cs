using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para filtrar y buscar registros de asistencia
    /// </summary>
    public record AssistanceFilterDto(
        string? SearchTerm,
        
        int? EmployeeId,
        
        int? ProjectId,
        
        DateTime? CheckInFrom,
        
        DateTime? CheckInTo,
        
        DateTime? CheckOutFrom,
        
        DateTime? CheckOutTo,
        
        string? RegisterType,
        
        bool? IsActive,
        
        bool? IsOvertime,
        
        bool? HasNotes,
        
        decimal? MinHours,
        
        decimal? MaxHours,
        
        DateTime? DateFrom,
        
        DateTime? DateTo,
        
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero")]
        int Page = 1,
        
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        int PageSize = 10,
        
        string? SortBy = "CheckIn",
        
        bool SortDescending = true
    );
}
