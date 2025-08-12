namespace WTB.API.Models.DTOs.ProjectMaintenance
{
    /// <summary>
    /// DTO para listados de mantenimientos de proyecto (optimizado para performance)
    /// </summary>
    public record ProjectMaintenanceListDto(
        int Id,
        string ProjectName,
        DateTime MaintenanceDate,
        string MaintenanceDescription,
        string EmployeeName,
        decimal? MaintenanceCost,
        string StateName,
        DateTime CreatedDate
    );
}
