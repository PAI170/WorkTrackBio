namespace WTB.API.Models.DTOs.ProjectWarranty
{
    /// <summary>
    /// DTO para mostrar detalles completos de una garantía de proyecto
    /// </summary>
    public record ProjectWarrantyDetailsDto(
        int Id,
        int IdProject,
        DateTime WarrantyDate,
        string WarrantyDescription,
        int MadeById,
        decimal? WarrantyCost,
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
