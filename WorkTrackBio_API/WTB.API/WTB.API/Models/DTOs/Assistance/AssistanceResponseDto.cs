namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para respuesta con información completa de asistencia
    /// </summary>
    public record AssistanceResponseDto(
        int Id,
        int EmployeeId,
        string EmployeeName,
        string EmployeeDocumentNumber,
        int ProjectId,
        string ProjectName,
        DateTime CheckIn,
        DateTime? CheckOut,
        decimal? TotalHours,
        string? Notes,
        string RegisterType,
        DateTime CreatedDate,
        DateTime? ModifiedDate,
        DateTime CheckInDateOnly,
        
        // Información del dispositivo
        int? DeviceId,
        string? DeviceName,
        string? DeviceLocation,
        
        // Campos calculados
        bool IsActive,
        bool IsOvertime,
        decimal? RegularHours,
        decimal? OvertimeHours,
        TimeSpan? CurrentDuration,
        string Status
    );
}
