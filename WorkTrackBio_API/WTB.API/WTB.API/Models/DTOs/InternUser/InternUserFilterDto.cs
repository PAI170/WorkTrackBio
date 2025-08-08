using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para filtrar y buscar usuarios administrativos
    /// </summary>
    public record InternUserFilterDto(
        string? SearchTerm,
        
        string? Email,
        
        string? FirstName,
        
        string? LastName,
        
        int? RolId,
        
        int? StateId,
        
        bool? IsActive,
        
        DateTime? CreatedFrom,
        
        DateTime? CreatedTo,
        
        DateTime? LastLoginFrom,
        
        DateTime? LastLoginTo,
        
        bool? HasLoggedIn,
        
        int? MinAuditEntries,
        
        int? MaxAuditEntries,
        
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero")]
        int Page = 1,
        
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        int PageSize = 10,
        
        string? SortBy = "CreationDate",
        
        bool SortDescending = true
    );
}
