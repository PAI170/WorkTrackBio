namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para listados de dispositivos (optimizado para performance)
    /// </summary>
    public record DeviceListDto(
        int Id,
        string DeviceId,
        string DeviceName,
        string ProjectName,
        string? Location,
        string DeviceType,
        bool IsActive,
        DateTime CreatedDate,
        DateTime? LastSeen
    );
}
