namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para estadísticas de asignaciones de proyectos
    /// </summary>
    public record ProjectAssignStatsDto(
        int TotalAssignments,
        int ActiveAssignments,
        int CompletedAssignments,
        int OverdueAssignments,
        decimal TotalHoursWorked,
        decimal AverageHoursPerAssignment,
        int TotalEmployees,
        int TotalProjects
    );
}
