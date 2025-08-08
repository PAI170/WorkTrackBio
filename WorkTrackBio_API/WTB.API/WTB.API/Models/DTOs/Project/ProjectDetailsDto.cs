namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO para vista detallada del proyecto con información relacionada
    /// </summary>
    public record ProjectDetailsDto(
        int Id,
        string ProjectName,
        DateTime? StartDate,
        DateTime? EndDate,
        int DurationInDays,
        int StateId,
        string StateName,
        bool IsActive,
        bool IsCompleted,
        bool IsOverdue,
        DateTime CreatedDate,
        
        // Empleados asignados
        ICollection<ProjectEmployeeDto> AssignedEmployees,
        
        // Estadísticas detalladas
        ProjectStatsDto Stats,
        
        // Actividades recientes
        ICollection<ProjectActivityDto> RecentActivities
    );

    /// <summary>
    /// DTO para empleados asignados al proyecto
    /// </summary>
    public record ProjectEmployeeDto(
        int EmployeeId,
        string FullName,
        string DocumentNumber,
        DateTime AssignDate,
        DateTime? EndDate,
        bool IsActive,
        decimal HoursWorked,
        DateTime? LastActivity
    );

    /// <summary>
    /// DTO para estadísticas del proyecto
    /// </summary>
    public record ProjectStatsDto(
        int TotalEmployeesAssigned,
        int ActiveEmployees,
        int CompletedEmployees,
        decimal TotalHoursWorked,
        decimal AverageHoursPerEmployee,
        DateTime? LastActivity,
        decimal? ProgressPercentage,
        int TotalAssistanceRecords,
        decimal? EstimatedCost,
        decimal? ActualCost
    );

    /// <summary>
    /// DTO para actividades recientes del proyecto
    /// </summary>
    public record ProjectActivityDto(
        int Id,
        string ActivityType,
        string Description,
        string EmployeeName,
        DateTime ActivityDate,
        decimal? Hours
    );
}
