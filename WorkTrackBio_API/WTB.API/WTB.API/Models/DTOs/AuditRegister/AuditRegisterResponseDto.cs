namespace WTB.API.Models.DTOs.AuditRegister
{
    /// <summary>
    /// DTO de respuesta estándar para registros de auditoría
    /// </summary>
    public record AuditRegisterResponseDto(
        int Id,
        int AssistanceId,
        string ActionType,
        string DetailChange,
        DateTime ActionDate,
        int AdminId,
        string AdminName
    );
}
