namespace WTB.API.Models.DTOs.FingerPrint
{
    /// <summary>
    /// DTO para listados de huellas dactilares (optimizado para performance)
    /// </summary>
    public record FingerPrintListDto(
        int Id,
        int EmployeeId,
        string EmployeeName,
        string EmployeeDocumentNumber,
        DateTime IssueDate,
        DateTime CreatedDate
    );
}
