namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO de respuesta para solicitudes de acceso a dispositivos
    /// </summary>
    public record DeviceAccessResponseDto(
        bool IsAccessGranted,
        string Message,
        int? AssistanceId = null,
        DateTime? AccessTime = null,
        string? ProjectName = null,
        string? EmployeeName = null,
        string? DeviceName = null
    );
}
