namespace WTB.API.Models.DTOs.ProjectWarranty
{
    /// <summary>
    /// DTO para listados de garantías de proyecto (optimizado para performance)
    /// </summary>
    public record ProjectWarrantyListDto(
        int Id,
        string ProjectName,
        DateTime WarrantyDate,
        string WarrantyDescription,
        string EmployeeName,
        decimal? WarrantyCost,
        string StateName,
        DateTime CreatedDate
    );
}
