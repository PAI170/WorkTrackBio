namespace WTB.API.Models.DTOs.FingerPrint
{
    /// <summary>
    /// DTO de respuesta estándar para huellas dactilares
    /// </summary>
    public record FingerPrintResponseDto(
        int Id,
        int EmployeeId,
        string EmployeeName,
        DateTime IssueDate,
        DateTime CreatedDate
    );
}
