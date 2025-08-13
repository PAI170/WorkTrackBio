namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para respuesta con información de asignación empleado-proyecto
    /// </summary>
    public record ProjectAssignResponseDto(
        int Id,
        int EmployeeId,
        string EmployeeFullName,
        string EmployeeEmail,
        int ProjectId,
        string ProjectName,
        string ProjectDescription,
        DateTime AssignDate,
        DateTime? EndDate,
        
        // Campos calculados
        bool IsActive,
        int DaysAssigned,
        int? DaysRemaining,
        string Status,
        bool IsOverdue,
        decimal? TotalHoursWorked,
        decimal? AverageHoursPerDay,
        
        // Información del proyecto
        string ProjectState,
        DateTime? ProjectEndDate,
        bool ProjectIsActive
    );
}




