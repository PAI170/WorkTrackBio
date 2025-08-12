namespace WTB.API.Models.DTOs.AuditRegister
{
    /// <summary>
    /// DTO para mostrar detalles completos de un registro de auditoría
    /// </summary>
    public record AuditRegisterDetailsDto(
        int Id,
        int AssistanceId,
        string ActionType,
        string DetailChange,
        DateTime ActionDate,
        int AdminId,
        
        // Información de la asistencia relacionada
        string EmployeeName,
        string ProjectName,
        DateTime AssistanceCheckIn,
        DateTime? AssistanceCheckOut,
        
        // Información del administrador
        string AdminName,
        string AdminEmail
    );
}
