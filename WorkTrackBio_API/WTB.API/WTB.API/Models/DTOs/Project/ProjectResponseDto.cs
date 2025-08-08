namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO para respuesta con información completa del proyecto
    /// </summary>
    public record ProjectResponseDto(
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
        
        // Estadísticas del proyecto
        int TotalEmployeesAssigned,
        int ActiveEmployees,
        decimal TotalHoursWorked,
        DateTime? LastActivity,
        decimal? ProjectProgress
    );
}
