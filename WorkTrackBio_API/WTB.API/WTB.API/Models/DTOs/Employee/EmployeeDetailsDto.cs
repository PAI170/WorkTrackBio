namespace WTB.API.Models.DTOs.Employee
{
    /// <summary>
    /// DTO para vista detallada del empleado con información relacionada
    /// </summary>
    public record EmployeeDetailsDto(
        int Id,
        string DocumentNumber,
        int DocumentTypeId,
        string DocumentTypeName,
        DateTime? DocumentExpire,
        string FirstName,
        string LastName,
        string FullName,
        string? PhoneNumber,
        string? EmergencyContact,
        string? EmergencyContactPhoneNumber,
        DateTime Birthday,
        int Age,
        DateTime RegisterDate,
        decimal? CostPerHour,
        int StateId,
        string StateName,
        string? Address,
        string? IBAN,
        bool HasFingerPrint,
        
        // Información relacionada
        ICollection<ProjectAssignmentDto> ProjectAssignments,
        EmployeeStatsDto Stats
    );

    /// <summary>
    /// DTO para asignaciones de proyecto del empleado
    /// </summary>
    public record ProjectAssignmentDto(
        int ProjectId,
        string ProjectName,
        DateTime AssignDate,
        DateTime? EndDate,
        bool IsActive,
        string ProjectState
    );

    /// <summary>
    /// DTO para estadísticas del empleado
    /// </summary>
    public record EmployeeStatsDto(
        int TotalProjects,
        int ActiveProjects,
        int CompletedProjects,
        decimal TotalHoursWorked,
        DateTime? LastAssistance,
        int TotalAssistanceRecords
    );
}
