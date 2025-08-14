namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para respuesta con información básica de asistencia
    /// </summary>
    public record AssistanceResponseDto(
        int Id,
        int EmployeeId,
        string EmployeeName,
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
        int? DeviceId
    );
}
