namespace WTB.API.Models.DTOs.FingerPrint
{
    /// <summary>
    /// DTO para mostrar detalles completos de una huella dactilar
    /// </summary>
    public record FingerPrintDetailsDto(
        int Id,
        int EmployeeId,
        byte[] TemplateFingerPrint,
        DateTime IssueDate,
        DateTime CreatedDate,
        DateTime? ModifiedDate,
        
        // Información del empleado
        string EmployeeFirstName,
        string EmployeeLastName,
        string EmployeeDocumentNumber,
        
        // Información de auditoría
        int? CreatedById,
        int? ModifiedById,
        string? CreatedBy,
        string? ModifiedBy
    );
}
