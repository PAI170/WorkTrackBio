namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO simplificado para listados de asignaciones empleado-proyecto
    /// </summary>
    public record ProjectAssignListDto(
        int Id,
        string EmployeeFullName,
        string ProjectName,
        DateTime AssignDate,
        DateTime? EndDate,
        bool IsActive,
        string Status,
        int DaysAssigned,
        decimal? TotalHoursWorked
    );
}






