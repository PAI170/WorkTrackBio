namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para respuesta con información de usuario administrativo
    /// </summary>
    public record InternUserResponseDto(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string FullName,
        int RolId,
        string RolName,
        int StateId,
        string StateName,
        DateTime CreationDate,
        DateTime? LastLogin,
        
        // Campos calculados
        bool IsActive,
        int DaysSinceCreation,
        int? DaysSinceLastLogin,
        string Status,
        bool CanLogin,
        
        // Estadísticas de actividad
        int TotalAuditEntries,
        DateTime? LastAuditAction
    );
}
