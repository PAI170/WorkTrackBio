namespace WTB.API.Models.DTOs.ProjectMaintenance
{
    /// <summary>
    /// DTO de respuesta estándar para mantenimientos de proyecto
    /// </summary>
    public record ProjectMaintenanceResponseDto(
        int Id,
        int IdProject,
        string ProjectName,
        DateTime MaintenanceDate,
        string MaintenanceDescription,
        string EmployeeName,
        decimal? MaintenanceCost,
        string StateName,
        DateTime CreatedDate
    );
}
