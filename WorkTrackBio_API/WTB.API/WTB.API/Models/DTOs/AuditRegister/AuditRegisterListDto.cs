namespace WTB.API.Models.DTOs.AuditRegister
{
    /// <summary>
    /// DTO para listados de registros de auditoría (optimizado para performance)
    /// </summary>
    public record AuditRegisterListDto(
        int Id,
        string ActionType,
        DateTime ActionDate,
        string EmployeeName,
        string ProjectName,
        string AdminName,
        string DetailChange
    );
}
