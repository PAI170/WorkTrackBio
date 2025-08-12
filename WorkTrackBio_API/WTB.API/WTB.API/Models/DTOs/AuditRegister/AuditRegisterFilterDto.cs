using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.AuditRegister
{
    /// <summary>
    /// DTO para filtrar y buscar registros de auditoría
    /// </summary>
    public record AuditRegisterFilterDto(
        // Filtros por fecha
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        
        // Filtros por tipo de acción
        string? ActionType = null,
        
        // Filtros por asistencia
        int? AssistanceId = null,
        int? EmployeeId = null,
        int? ProjectId = null,
        
        // Filtros por administrador
        int? AdminId = null,
        
        // Filtros de texto
        string? SearchText = null,
        
        // Paginación
        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        int PageSize = 20,
        
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        int PageNumber = 1
    );
}
