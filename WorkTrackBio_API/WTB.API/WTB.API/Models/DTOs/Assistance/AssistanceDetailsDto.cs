namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para vista detallada de asistencia con información relacionada
    /// </summary>
    public record AssistanceDetailsDto(
        int Id,
        int EmployeeId,
        string EmployeeName,
        string EmployeeDocumentNumber,
        int ProjectId,
        string ProjectName,
        DateTime CheckIn,
        DateTime? CheckOut,
        decimal? TotalHours,
        string? Notes,
        string RegisterType,
        DateTime CreatedDate,
        DateTime? ModifiedDate,
        DateTime CheckInDateOnly,
        
        // Información detallada
        EmployeeInfoDto Employee,
        ProjectInfoDto Project,
        AssistanceStatsDto Stats,
        
        // Auditoría
        ICollection<AssistanceAuditDto> AuditTrail
    );

    /// <summary>
    /// DTO con información básica del empleado para asistencias
    /// </summary>
    public record EmployeeInfoDto(
        int Id,
        string FullName,
        string DocumentNumber,
        decimal? CostPerHour,
        string? PhoneNumber
    );

    /// <summary>
    /// DTO con información básica del proyecto para asistencias
    /// </summary>
    public record ProjectInfoDto(
        int Id,
        string ProjectName,
        DateTime? StartDate,
        DateTime? EndDate,
        string StateName
    );

    /// <summary>
    /// DTO para estadísticas de la asistencia
    /// </summary>
    public record AssistanceStatsDto(
        decimal? RegularHours,
        decimal? OvertimeHours,
        decimal? EstimatedCost,
        bool IsLateCheckIn,
        bool IsEarlyCheckOut,
        bool IsOvertime,
        TimeSpan? BreakTime,
        decimal? Efficiency
    );

    /// <summary>
    /// DTO para auditoría de cambios en asistencia
    /// </summary>
    public record AssistanceAuditDto(
        int Id,
        string ActionType,
        string DetailChange,
        DateTime ActionDate,
        string AdminName
    );
}






