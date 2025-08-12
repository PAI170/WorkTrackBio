namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO de respuesta estándar para dispositivos
    /// </summary>
    public record DeviceResponseDto(
        int Id,
        string DeviceId,
        string DeviceName,
        int ProjectId,
        string? Location,
        string DeviceType,
        bool IsActive,
        DateTime CreatedDate,
        DateTime? LastSeen
    );
}
