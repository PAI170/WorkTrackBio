using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para filtrar y buscar dispositivos
    /// </summary>
    public record DeviceFilterDto(
        // Filtros por proyecto
        int? ProjectId = null,
        
        // Filtros por estado
        bool? IsActive = null,
        
        // Filtros por tipo
        string? DeviceType = null,
        
        // Filtros por ubicación
        string? Location = null,
        
        // Filtros de texto
        string? SearchText = null,
        
        // Filtros por fecha
        DateTime? CreatedFrom = null,
        DateTime? CreatedTo = null,
        DateTime? LastSeenFrom = null,
        DateTime? LastSeenTo = null,
        
        // Paginación
        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        int PageSize = 20,
        
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        int PageNumber = 1
    );
}
