namespace WTB.API.Models.DTOs.ProjectWarranty
{
    /// <summary>
    /// DTO de respuesta estándar para garantías de proyecto
    /// </summary>
    public record ProjectWarrantyResponseDto(
        int Id,
        int IdProject,
        string ProjectName,
        DateTime WarrantyDate,
        string WarrantyDescription,
        string EmployeeName,
        decimal? WarrantyCost,
        string StateName,
        DateTime CreatedDate
    );
}
