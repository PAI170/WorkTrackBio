namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para vista detallada de asignación empleado-proyecto
    /// </summary>
    public record ProjectAssignDetailsDto(
        int Id,
        int EmployeeId,
        string EmployeeFullName,
        string EmployeeEmail,
        string EmployeePhone,
        string EmployeeDocumentNumber,
        int ProjectId,
        string ProjectName,
        string ProjectDescription,
        DateTime AssignDate,
        DateTime? EndDate,
        
        // Información detallada del empleado
        EmployeeInfoDto Employee,
        
        // Información detallada del proyecto
        ProjectInfoDto Project,
        
        // Estadísticas de trabajo
        AssignmentStatsDto AssignmentStats,
        
        // Historial de asistencia (últimas 10 entradas)
        ICollection<AssignmentAssistanceDto> RecentAssistances
    );

    /// <summary>
    /// DTO con información del empleado para asignaciones
    /// </summary>
    public record EmployeeInfoDto(
        int Id,
        string FullName,
        string Email,
        string Phone,
        string DocumentNumber,
        string DocumentType,
        string State,
        decimal? CostPerHour,
        DateTime HireDate,
        bool IsActive
    );

    /// <summary>
    /// DTO con información del proyecto para asignaciones
    /// </summary>
    public record ProjectInfoDto(
        int Id,
        string ProjectName,
        string Description,
        string State,
        DateTime StartDate,
        DateTime? EndDate,
        decimal? Budget,
        decimal? ActualCost,
        bool IsActive,
        bool IsCompleted,
        int TotalAssignedEmployees,
        decimal TotalProjectHours
    );

    /// <summary>
    /// DTO para estadísticas de la asignación
    /// </summary>
    public record AssignmentStatsDto(
        int DaysAssigned,
        int? DaysRemaining,
        decimal TotalHoursWorked,
        decimal AverageHoursPerDay,
        decimal? RegularHours,
        decimal? OvertimeHours,
        decimal? EstimatedCost,
        decimal? ActualCost,
        bool IsOverdue,
        string PerformanceStatus,
        decimal CompletionPercentage
    );

    /// <summary>
    /// DTO para asistencia relacionada con la asignación
    /// </summary>
    public record AssignmentAssistanceDto(
        int Id,
        DateTime CheckIn,
        DateTime? CheckOut,
        decimal? TotalHours,
        string RegisterType,
        string? Notes,
        DateTime CreatedDate,
        string Status
    );
}
