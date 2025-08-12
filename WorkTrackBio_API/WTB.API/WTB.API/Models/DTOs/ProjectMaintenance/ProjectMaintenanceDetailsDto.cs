namespace WTB.API.Models.DTOs.ProjectMaintenance
{
    /// <summary>
    /// DTO para mostrar detalles completos de un mantenimiento de proyecto
    /// </summary>
    public record ProjectMaintenanceDetailsDto(
        int Id,
        int IdProject,
        DateTime MaintenanceDate,
        string MaintenanceDescription,
        int MadeById,
        decimal? MaintenanceCost,
        string? AdditionalInfo,
        int StateId,
        DateTime CreatedDate,
        DateTime? ModifiedDate,
        
        // Información del proyecto
        string ProjectName,
        string ProjectState,
        
        // Información del empleado
        string EmployeeFirstName,
        string EmployeeLastName,
        string EmployeeDocumentNumber,
        
        // Información del estado
        string StateName,
        string StateType,
        
        // Información de auditoría
        int? CreatedById,
        int? ModifiedById,
        string? CreatedBy,
        string? ModifiedBy
    );
}
