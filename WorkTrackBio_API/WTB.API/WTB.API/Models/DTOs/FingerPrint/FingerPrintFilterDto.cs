using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.FingerPrint
{
    /// <summary>
    /// DTO para filtrar y buscar huellas dactilares
    /// </summary>
    public record FingerPrintFilterDto(
        // Filtros por empleado
        int? EmployeeId = null,
        string? EmployeeDocumentNumber = null,
        string? EmployeeName = null,
        
        // Filtros por fecha
        DateTime? IssueDateFrom = null,
        DateTime? IssueDateTo = null,
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
