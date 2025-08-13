namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para vista detallada de usuario administrativo
    /// </summary>
    public record InternUserDetailsDto(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string FullName,
        int RolId,
        string RoleName,
        int StateId,
        string StateName,
        DateTime CreationDate,
        DateTime? LastLogin,
        
        // Información detallada del rol
        RoleInfoDto Role,
        
        // Información del estado
        StateInfoDto State,
        
        // Estadísticas de actividad
        UserActivityStatsDto ActivityStats,
        
        // Historial de auditoría (últimas 10 acciones)
        ICollection<UserAuditDto> RecentAuditActions
    );

    /// <summary>
    /// DTO con información del rol para usuarios
    /// </summary>
    public record RoleInfoDto(
        int Id,
        string RoleName,
        string? Description,
        bool HasFullAccess,
        ICollection<string> Permissions
    );

    /// <summary>
    /// DTO con información del estado para usuarios
    /// </summary>
    public record StateInfoDto(
        int Id,
        string StateName,
        bool IsActive,
        string? Description
    );

    /// <summary>
    /// DTO para estadísticas de actividad del usuario
    /// </summary>
    public record UserActivityStatsDto(
        int TotalAuditEntries,
        int AuditEntriesThisMonth,
        int AuditEntriesToday,
        DateTime? LastAuditAction,
        string? LastAuditType,
        int DaysSinceCreation,
        int? DaysSinceLastLogin,
        bool IsActiveUser,
        decimal ActivityScore
    );

    /// <summary>
    /// DTO para acciones de auditoría del usuario
    /// </summary>
    public record UserAuditDto(
        int Id,
        string ActionType,
        string DetailChange,
        DateTime ActionDate,
        int AssistanceId,
        string? EmployeeName,
        string? ProjectName
    );
}




